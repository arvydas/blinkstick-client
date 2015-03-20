using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.IO.Pipes;

namespace BlinkStickClient.DataModel
{
    public class NotificationAmbilight : Notification
    {
        Process spawnedProcess;

        AutoResetEvent resetEvent = new AutoResetEvent(false);

        private BackgroundWorker ambilightWorker = null;

        public override string GetTypeName()
        {
            return "Ambilight";
        }

        public NotificationAmbilight()
        {
        }
    
        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationAmbilight();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0}", GetTypeName());

            base.Start();

            ambilightWorker = new BackgroundWorker();
            ambilightWorker.DoWork += AnalyzeBackgroundColor;
            ambilightWorker.WorkerSupportsCancellation = true;
            ambilightWorker.RunWorkerAsync();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            psi.Arguments = "--ambilight";
            psi.CreateNoWindow = true;

            spawnedProcess = Process.Start(psi);

            log.DebugFormat("{0} started", GetTypeName());
        }

        private static readonly int BufferSize = 3;

        protected virtual void AnalyzeBackgroundColor (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            Byte[] bytes = new Byte[BufferSize];
            char[] chars = new char[BufferSize];
            int numBytes = 0;
            try
            {
                NamedPipeServerStream pipeServer = new NamedPipeServerStream("blinkstick_ambilight_local", 
                    PipeDirection.In, 
                    1,
                    PipeTransmissionMode.Message, 
                    PipeOptions.Asynchronous);

                pipeServer.WaitForConnection();

                do
                {
                    do
                    {
                        numBytes = pipeServer.Read(bytes, 0, BufferSize);
                        if (numBytes > 0)
                        {
                            OnColorSend(bytes[0], bytes[1], bytes[2]);
                        }
                    } 
                    while (numBytes > 0 && !pipeServer.IsMessageComplete && pipeServer.IsConnected && !worker.CancellationPending);

                } 
                while (numBytes != 0 && !worker.CancellationPending);

                pipeServer.Disconnect();

                pipeServer.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            OnColorSend(0, 0, 0);

            resetEvent.Set();
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0}", GetTypeName());

            ambilightWorker.CancelAsync();

            resetEvent.WaitOne();

            base.Stop();
            log.DebugFormat("{0} stopped", GetTypeName());
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

    }
}

