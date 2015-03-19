using System;
using System.Diagnostics;
using System.ComponentModel;
using BlinkStickDotNet;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationCpu : HardwareNotification
    {
        public NotificationCpu()
        {
        }

        public override string GetTypeName()
        {
            return "CPU";
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationCpu();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        protected override void InitializePerformanceCounter (object sender, DoWorkEventArgs e)
        {
            log.Debug("Initializing CPU performance counter in background");
            PerformanceCounter counter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            //first value is always 0%
            counter.NextValue();
            //Start using the performance counter
            performanceCounter = counter;
            base.InitializePerformanceCounter(sender, e);
            log.Debug("CPU performance counter initialization done");
        }

        #region implemented abstract members of HardwareNotification

        public override int GetValue()
        {
            return Convert.ToInt32(performanceCounter.NextValue());
        }

        #endregion
    }
}

