using System;
using System.ComponentModel;
using SlimDX.Direct3D9;
using SlimDX;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;

namespace BlinkStickClient
{
    public class AmbilightWindowsService
    {
        private const int RefreshPeriod = 50;

        private BackgroundWorker ambilightWorker = null;

        DxScreenCapture sc;

        public AmbilightWindowsService()
        {
        }

        public void Run()
        {
            sc = new DxScreenCapture();

            /*
            var format = SharpDX.Direct3D9.Format.A8B8G8R8;

            SharpDX.Direct3D9.PresentParameters present_paramsS = new SharpDX.Direct3D9.PresentParameters();

            present_paramsS.Windowed = true;
            present_paramsS.SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard;

            SharpDX.Direct3D9.Device dS = new SharpDX.Direct3D9.Device(new SharpDX.Direct3D9.Direct3D(),
                0,
                SharpDX.Direct3D9.DeviceType.NullReference,
                IntPtr.Zero,
                SharpDX.Direct3D9.CreateFlags.SoftwareVertexProcessing,
                present_paramsS);
            */

            ambilightWorker = new BackgroundWorker();
            ambilightWorker.DoWork += AnalyzeBackgroundColor;
            ambilightWorker.WorkerSupportsCancellation = true;
            ambilightWorker.RunWorkerAsync();

            while (ambilightWorker.IsBusy)
            {
                Thread.Sleep(10);
            }
        }

        protected virtual void AnalyzeBackgroundColor (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "blinkstick_ambilight_local",
                                                          PipeDirection.Out,
                                                          PipeOptions.Asynchronous))
            {
                try
                {
                    pipeClient.Connect(2000);
                }
                catch
                {
                    return;
                }

                while (!worker.CancellationPending)
                {
                    Surface s = sc.CaptureScreen();
                    DataRectangle dr = s.LockRectangle(LockFlags.None);
                    DataStream gs = dr.Data;

                    byte[] bu = new byte[4];
                    uint r = 0;
                    uint g = 0;
                    uint b = 0;
                    const int Bpp = 4; //Bytes per pixel

                    int count = 0;

                    int step = 4;

                    for (int j = 0; j < Screen.PrimaryScreen.Bounds.Height / step; j++)
                    {
                        for (int i = 0; i < Screen.PrimaryScreen.Bounds.Width / step; i++)
                        {
                            gs.Position = (j * step * Screen.PrimaryScreen.Bounds.Height + i * step) * Bpp;

                            gs.Read(bu, 0, 4);

                            r += bu[2];
                            g += bu[1];
                            b += bu[0];

                            count++;
                        }
                    }

                    s.UnlockRectangle();
                    s.Dispose();

                    if (pipeClient.IsConnected)
                    {
                        pipeClient.Write(new byte[] { (byte)(r / count), (byte)(g / count), (byte)(b / count) }, 0, 3);
                    }
                    else
                    {
                        return;
                    }

                    Thread.Sleep(RefreshPeriod);
                }
            }
        }

        public class DxScreenCapture
        {
            Device d;

            public DxScreenCapture()
            {
                PresentParameters present_params = new PresentParameters();
                present_params.Windowed = true;
                present_params.SwapEffect = SwapEffect.Discard;
                d = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, present_params);
            }

            public Surface CaptureScreen()
            {
                Surface s = Surface.CreateOffscreenPlain(d, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
                d.GetFrontBufferData(0, s);
                return s;
            }
        }

    }
}

