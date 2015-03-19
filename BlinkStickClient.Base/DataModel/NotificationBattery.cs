using System;
using System.ComponentModel;
using System.Windows.Forms;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationBattery : HardwareNotification
    {
        public override string GetTypeName()
        {
            return "Battery";
        }

        public NotificationBattery()
        {
            IsInitialized = true;
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBattery();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        #region implemented abstract members of HardwareNotification

        public override int GetValue()
        {
            if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
            {
                throw new Exception("No battery");
            }

            return (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);
        }

        #endregion

        public override void Start()
        {
            if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
            {
                log.Info("Unable to start monitoring, no battery detected"); 
                return;
            }

            base.Start();
        }
    }
}

