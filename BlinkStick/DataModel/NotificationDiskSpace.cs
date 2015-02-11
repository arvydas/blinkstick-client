using System;
using System.IO;
using System.Collections.Generic;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationDiskSpace : PatternNotification
    {
        private ILog log;  

        public String Drive { get; set; }

        public int DriveFreeSpaceLimit { get; set; }

        public MeasurementEnum Measurement { get; set; }

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        private long DriveFreeSpaceLimitValue
        {
            get
            {
                long result = 0;
                switch (Measurement)
                {
                    case MeasurementEnum.MB:
                        result = 1024 * 1024;
                        break;
                    case MeasurementEnum.GB:
                        result = 1024 * 1024 * 1024;
                        break;
                    default:
                        break;
                }

                result = result * DriveFreeSpaceLimit;

                return result;
            }
        }

        public override string GetTypeName()
        {
            return "Disk Space";
        }

        public NotificationDiskSpace()
        {
            this.Drive = "";
            this.DriveFreeSpaceLimit = 1;
            this.Measurement = MeasurementEnum.GB;
            this.CheckPeriod = 1;
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationDiskSpace();
            }

            ((NotificationDiskSpace)notification).Drive = this.Drive;
            ((NotificationDiskSpace)notification).DriveFreeSpaceLimit = this.DriveFreeSpaceLimit;
            ((NotificationDiskSpace)notification).Measurement = this.Measurement;
            ((NotificationDiskSpace)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        public override Gtk.Widget GetEditorWidget()
        {
            return new DiskSpaceEditorWidget();
        }

        public string[] GetDrives()
        {
            List<string> drives = new List<string>();
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                drives.Add(d.Name);
            }

            return drives.ToArray();
        }

        public override void Start()
        {
            if (Running)
                return;

            if (log == null)
            {
                log = LogManager.GetLogger(String.Format("{0}:{1}", GetTypeName(), this.Name));
            }

            log.InfoFormat("Starting {0} monitoring", GetTypeName());

            base.Start();

#if DEBUG
            //Faster checks for debugging
            int timeFrame = 10000;
#else
            int timeFrame = 60000;
#endif
            log.DebugFormat("Starting timer at {0} ms", (uint)(this.CheckPeriod * timeFrame));
            RefreshTimer = GLib.Timeout.Add((uint)(this.CheckPeriod * timeFrame), new GLib.TimeoutHandler(CheckUsage));

            log.DebugFormat("{0} monitoring started", GetTypeName());
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0} monitoring", GetTypeName());

            //Stop timer
            GLib.Source.Remove(RefreshTimer);

            base.Stop();
            log.DebugFormat("{0} monitoring stopped", GetTypeName());
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        protected bool CheckUsage()
        {
            DriveInfo di = new DriveInfo(this.Drive);

            if (!di.IsReady)
            {
                log.WarnFormat("Device {0} is not ready", this.Drive);
            }

            log.DebugFormat("Disk space reading {0}", HumanSize(di.TotalFreeSpace));

            if (di.TotalFreeSpace < this.DriveFreeSpaceLimitValue)
            {
                log.InfoFormat("Trigger notification rule ({0} < {1})", HumanSize(di.TotalFreeSpace), HumanSize(this.DriveFreeSpaceLimitValue));
                OnTriggered(String.Format("Drive {0} space {1} below {2}", this.Drive, HumanSize(di.TotalFreeSpace), HumanSize(this.DriveFreeSpaceLimitValue)));
            }
                
            return true;
        }

        public override void Dispose()
        {
            Stop();
            base.Dispose();
        }

        private String HumanSize(long value)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (value >= 1024 && order + 1 < sizes.Length) {
                order++;
                value = value/1024;
            }

            return String.Format("{0:0.##}{1}", value, sizes[order]);
        }
    }

    public enum MeasurementEnum
    {
        MB,
        GB
    }
}

