using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlinkStickClient.DataModel
{
    public class NotificationRam : HardwareNotification
    {
        public override string GetTypeName()
        {
            return "RAM";
        }

        public NotificationRam()
        {
            IsInitialized = true;
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationRam();
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
			if (HidSharp.PlatformDetector.RunningPlatform () == HidSharp.PlatformDetector.Platform.Windows) 
			{
				Int64 phav = GetPhysicalAvailableMemoryInMiB ();
				Int64 tot = GetTotalMemoryInMiB ();
				decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
				decimal percentOccupied = 100 - percentFree;

				return (int)percentOccupied;
			}
			else if (HidSharp.PlatformDetector.RunningPlatform () == HidSharp.PlatformDetector.Platform.Mac) 
			{
				var psi = new ProcessStartInfo(global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "scripts", "osx-ram.sh"))
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

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }
}

