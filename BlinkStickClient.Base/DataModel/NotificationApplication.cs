using System;
using BlinkStickClient.Utils;
using System.IO;

namespace BlinkStickClient.DataModel
{
    public class NotificationApplication : PatternNotification
    {
        public String SearchString { get; set; }

        public static int WindowMonitorRefCounter = 0;

        public static ActiveWindowMonitor _WindowMonitor;

        public ActiveWindowMonitor WindowMonitor
        {
            get
            {
                if (_WindowMonitor == null)
                {
                    _WindowMonitor = new ActiveWindowMonitor();
                }

                return _WindowMonitor;
            }
        }

        public override string GetTypeName()
        {
            return "Application";
        }

        public NotificationApplication()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationApplication();
            }

            ((NotificationApplication)notification).SearchString = this.SearchString;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0} monitoring", GetTypeName());

            WindowMonitor.ProcessChanged += ProcessChanged;
            WindowMonitor.WindowTextChanged += WindowTextChanged;

            WindowMonitorRefCounter++;

            if (!WindowMonitor.Running)
            {
                WindowMonitor.Start();
            }

            base.Start();

            log.DebugFormat("{0} monitoring started", GetTypeName());
        }

        void WindowTextChanged (object sender, WindowTextChangedEventArgs e)
        {
            if (e.WindowText.ToLower().Contains(this.SearchString.ToLower()))
            {
                OnTriggered(String.Format("{0} found in {1}", this.SearchString, e.WindowText));
            }
        }

        void ProcessChanged (object sender, ProcessChangedEventArgs e)
        {
            if (Path.GetFileName(e.ExecutableFileName).ToLower().Contains(this.SearchString.ToLower()))
            {
                OnTriggered(String.Format("{0} found in {1}", this.SearchString, Path.GetFileName(e.ExecutableFileName)));
            }
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0} monitoring", GetTypeName());

            WindowMonitor.ProcessChanged -= ProcessChanged;

            WindowMonitorRefCounter--;

            if (WindowMonitorRefCounter == 0)
            {
                WindowMonitor.Stop();
            }

            base.Stop();
            log.DebugFormat("{0} monitoring stopped", GetTypeName());
        }
    }
}

