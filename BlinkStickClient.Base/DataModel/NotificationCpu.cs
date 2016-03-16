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

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationCpu();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
			return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows || 
				HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Mac;
        }

        protected override void InitializePerformanceCounter (object sender, DoWorkEventArgs e)
        {
			if (HidSharp.PlatformDetector.RunningPlatform () == HidSharp.PlatformDetector.Platform.Windows) 
			{
				log.Debug ("Initializing CPU performance counter in background");
				PerformanceCounter counter = new PerformanceCounter ("Processor", "% Processor Time", "_Total", true);
				//first value is always 0%
				counter.NextValue ();
				//Start using the performance counter
				performanceCounter = counter;
				log.Debug ("CPU performance counter initialization done");
			}

			base.InitializePerformanceCounter (sender, e);
        }

        #region implemented abstract members of HardwareNotification

        public override int GetValue()
        {
			if (HidSharp.PlatformDetector.RunningPlatform () == HidSharp.PlatformDetector.Platform.Windows) 
			{
				return Convert.ToInt32(performanceCounter.NextValue());
			} 
			else if (HidSharp.PlatformDetector.RunningPlatform () == HidSharp.PlatformDetector.Platform.Mac) 
			{
				var psi = new ProcessStartInfo(global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "scripts", "osx-cpu.sh"))
				{
					RedirectStandardOutput = true,
					UseShellExecute = false
				};
				Process p = Process.Start(psi);
				string outString = p.StandardOutput.ReadToEnd();
				p.WaitForExit();
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
    }
}

