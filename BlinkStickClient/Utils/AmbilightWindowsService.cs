using System;
using System.ComponentModel;
using SlimDX.Direct3D9;
using SlimDX;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;
using log4net;
using Capture.Interface;
using Capture.Hook;
using Capture;
using System.Diagnostics;
using System.IO;

namespace BlinkStickClient.Utils
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

        Boolean ignorePipeError = true;

        FullScreenMonitor fullScreenMonitor;

        private const int RefreshPeriod = 50;

        private BackgroundWorker ambilightWorker = null;

        DxScreenCapture sc;

        private CaptureInterface captureInterface;
        private CaptureProcess captureProcess;

        private uint HookedProcessId = 0;

        private CaptureModeEnum _CaptureMode = CaptureModeEnum.Desktop;
        private CaptureModeEnum CaptureMode
        {
            get
            {
                return _CaptureMode;
            }
            set
            {
                if (_CaptureMode != value)
                {
                    _CaptureMode = value;

                    log.InfoFormat("Capture mode changed to {0}", value);
                }
            }
        }

        private enum CaptureModeEnum
        {
            None,
            Desktop,
            Application
        }

        public AmbilightWindowsService()
        {
        }

        public void Run()
        {
            log.Info("Initializing DirectX screen capture");
            sc = new DxScreenCapture();

            fullScreenMonitor = new FullScreenMonitor();
            fullScreenMonitor.Changed += HandleFullScreenApplicationChanged;
            fullScreenMonitor.Start();

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
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(10);
            }

            DetachFromHookedProcess();

            log.Info("Stopping active window monitor");
            fullScreenMonitor.Stop();
            log.Info("Service stopped");
        }

        void HandleFullScreenApplicationChanged (object sender, ChangedEventArgs e)
        {
            Process p = Process.GetProcessById((int)e.ProcessId);
            String processFileName = "";

            /*
            if (e.ProcessId != 0)
            {
                processFileName = Path.GetFileName(p.MainModule.FileName).ToLower();
            }
            */

            if (e.FullScreen && !processFileName.Contains("powerptn.exe"))
            {
                if (HookedProcessId != e.ProcessId)
                {
                    //Disable capture while hooking into process
                    CaptureMode = CaptureModeEnum.None;

                    HookedProcessId = e.ProcessId;

                    if (HookManager.IsHooked((int)e.ProcessId))
                    {
                        log.InfoFormat("Process {0} is already hooked", HookedProcessId);
                    }
                    else
                    {
                        log.InfoFormat("Hooking into {0}", HookedProcessId);

                        CaptureConfig cc = new CaptureConfig()
                        {
                            Direct3DVersion = Direct3DVersion.AutoDetect,
                            ShowOverlay = false
                        };

                        captureInterface = new CaptureInterface();
                        captureInterface.RemoteMessage += HandleRemoteMessage;

                        Process process = Process.GetProcessById((int)HookedProcessId);

                        log.InfoFormat("Starting {0}:{1} process capture", HookedProcessId, process.MainWindowTitle);
                        try
                        {
                            captureProcess = new CaptureProcess(process, cc, captureInterface);
                            log.InfoFormat("Hooking into {0} complete", HookedProcessId);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Failed to hook into process {0}", ex);
                        }
                    }
                }

                CaptureMode = CaptureModeEnum.Application;
            }
            else
            {
                CaptureMode = CaptureModeEnum.Desktop;

                DetachFromHookedProcess();
            }
        }

        void HandleRemoteMessage (MessageReceivedEventArgs message)
        {
            switch (message.MessageType)
            {
                case MessageType.Debug:
                    log.Debug(message.Message);
                    break;
                case MessageType.Error:
                    log.Error(message.Message);
                    break;
                case MessageType.Information:
                    log.Info(message.Message);
                    break;
                case MessageType.Warning:
                    log.Warn(message.Message);
                    break;
            }
        }

        protected virtual void AnalyzeBackgroundColor (object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = (BackgroundWorker)sender;

                log.Info("Creating pipe");
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "blinkstick_ambilight_local",
                                                       PipeDirection.Out,
                                                       PipeOptions.Asynchronous);
                while (!pipeClient.IsConnected && !worker.CancellationPending)
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
                        if (!ignorePipeError)
                        {
                            return;
                        }
                    }
                }

                while (!worker.CancellationPending)
                {
                    if (CaptureMode == CaptureModeEnum.Desktop)
                    {
                        Surface s = null;
                        try
                        {
                            s = sc.CaptureScreen();
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("Failed to capture screen {0}", ex);
                        }

                        if (s != null)
                        {
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
                                    gs.Position = (j * step * Screen.PrimaryScreen.Bounds.Width + i * step) * Bpp;

                                    gs.Read(bu, 0, 4);

                                    r += bu[2];
                                    g += bu[1];
                                    b += bu[0];

                                    count++;
                                }
                            }

                            s.UnlockRectangle();
                            s.Dispose();

                            if (pipeClient != null)
                            {
                                if (pipeClient.IsConnected)
                                {
                                    log.DebugFormat("Color: {0:X2}{1:X2}{2:X2}", (byte)(r / count), (byte)(g / count), (byte)(b / count));
                                    pipeClient.Write(new byte[] { (byte)(r / count), (byte)(g / count), (byte)(b / count) }, 0, 3);
                                }
                                else
                                {
                                    if (!ignorePipeError)
                                    {
                                        log.Info("Client disconnected. Exiting...");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else if (CaptureMode == CaptureModeEnum.Application)
                    {
                        if (!ignorePipeError && pipeClient != null && !pipeClient.IsConnected)
                        {
                            log.Info("Client disconnected. Exiting...");
                            return;
                        }

                        if (captureProcess == null)
                        {
                            log.Error("Capture process not initialized, falling back to desktop");
                            CaptureMode = CaptureModeEnum.Desktop;
                        }
                        else
                        {
                            log.Info("Request on top");
                            captureProcess.BringProcessWindowToFront();

                            log.Info("Begin screenshot");
                            captureProcess.CaptureInterface.BeginGetScreenshot(
                                new System.Drawing.Rectangle(0, 0, 0, 0), 
                                new TimeSpan(0, 0, 2), 
                                //Callback, 
                                (IAsyncResult Result) => {
                                    try
                                    {
                                        if (captureProcess == null)
                                            return;

                                        using (Screenshot screenshot = captureProcess.CaptureInterface.EndGetScreenshot(Result))
                                        {
                                            if (screenshot == null)
                                            {
                                                log.Info("Callback received: null");
                                            }
                                            else
                                            {
                                                log.DebugFormat("Callback received: {0:X2},{1:X2},{2:X2}", (byte)screenshot.R, (byte)screenshot.G, (byte)screenshot.B);
                
                                                if (pipeClient != null && pipeClient.IsConnected)
                                                {
                                                    pipeClient.Write(new byte[] { screenshot.R, screenshot.G, screenshot.B }, 0, 3);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        log.ErrorFormat("Unable to process captured data {0}", ex);
                                    }
                                },
                                null, 
                                ImageFormat.AverageColor);
                        }
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
            catch (Exception ex)
            {
                log.ErrorFormat("Background color analyzer crash {0}", ex);
            }
        }

        void DetachFromHookedProcess()
        {
            if (HookedProcessId != 0)
            {
                log.InfoFormat("Detaching from hooked process {0}", HookedProcessId);

                if (captureProcess != null)
                {
                    captureProcess.CaptureInterface.Disconnect();
                }

                log.Info("Unhooking process");
                HookManager.RemoveHookedProcess((int)HookedProcessId);

                captureProcess = null;
                captureInterface = null;

                HookedProcessId = 0;
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

