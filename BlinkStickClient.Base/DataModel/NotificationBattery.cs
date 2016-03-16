using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
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

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBattery();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows ||
				HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Mac;
        }

        #region implemented abstract members of HardwareNotification

        public override int GetValue()
        {
			if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows)
			{
				if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
				{
					throw new Exception("No battery");
				}

				return (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100);
			}
			else if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Mac)
			{
				var psi = new ProcessStartInfo(global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "scripts", "osx-batt.sh"))
				{
					RedirectStandardOutput = true,
					UseShellExecute = false
				};
				Process p = Process.Start(psi);
				string outString = p.StandardOutput.ReadToEnd();
				p.WaitForExit();

				if (outString.Trim() == "")
				{
					throw new Exception("No battery or battery info could not be read");
				}

				try
				{
					return (int)Math.Round(Convert.ToDouble(outString.Trim ()));
				}
				catch (Exception e) {
					log.ErrorFormat ("Failed to convert string \"{0}\" to int: {1}", outString, e);
					return 0;
				}
			}

			return 0;
        }

        #endregion

        public override void Start()
        {
			if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows)
			{
				if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
	            {
	                log.Info("Unable to start monitoring, no battery detected"); 
	                return;
	            }
			}
			else if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Mac)
			{
				try
				{
					GetValue();
				}
				catch (Exception e)
				{
					log.InfoFormat("Unable to start monitoring: {0}", e.Message);
					return;
				}
			}

            base.Start();
        }
    }
}

