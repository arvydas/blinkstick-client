using System;
using System.ComponentModel;
using SlimDX.Direct3D9;
using SlimDX;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;
using log4net;

namespace BlinkStickClient
{
    public class AmbilightWindowsService
    {
        private ILog _log;

        protected ILog log 
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger("AmbilightWindowsService");
                }

                return _log;
            }
        }


        private const int RefreshPeriod = 50;

        private BackgroundWorker ambilightWorker = null;

        DxScreenCapture sc;

        public AmbilightWindowsService()
        {
        }

        public void Run()
        {
            log.Info("Initializing DirectX screen capture");
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

            log.Info("Starting background worker");

            ambilightWorker.RunWorkerAsync();

            while (ambilightWorker.IsBusy)
            {
                Thread.Sleep(10);
            }

            log.Info("Service stopped");
        }

        protected virtual void AnalyzeBackgroundColor (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            log.Info("Creating pipe");
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "blinkstick_ambilight_local",
                                                          PipeDirection.Out,
                                                          PipeOptions.Asynchronous))
            {
                try
                {
                    log.Info("Connecting to pipe");
                    pipeClient.Connect(2000);
                    log.Info("Conection to pipe established");
                }
                catch (Exception ex)
                {
                    log.Error("Failed to connect to pipe server: {0}", ex);
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
                        log.Info("Client disconnected. Exiting...");
                        return;
                    }

                    Thread.Sleep(RefreshPeriod);
                }

                log.Info("Worker cancelled. Exiting...");

                if (pipeClient.IsConnected)
                {
                    pipeClient.Close();
                    pipeClient.Dispose();
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

