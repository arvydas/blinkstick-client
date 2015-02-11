using System;
using System.Diagnostics;
using System.ComponentModel;
using BlinkStickDotNet;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationCpu : PatternNotification
    {
        protected ILog log;  

        #region Enums
        public enum TriggerTypeEnum
        {
            More,
            Less
        }

        public enum ConfigurationEnum
        {
            Monitor,
            Alert
        }
        #endregion

        public TriggerTypeEnum TriggerType
        {
            get;
            set;
        }

        public ConfigurationEnum Configuration { set; get; }

        public int AlertPercent { set; get; }

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        private static PerformanceCounter cpuCounter;

        private BackgroundWorker performanceCounterInitializer = null;

        public NotificationCpu()
        {
            this.TriggerType = TriggerTypeEnum.More;
            this.AlertPercent = 50;
            this.CheckPeriod = 1;
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

            ((NotificationCpu)notification).Configuration = this.Configuration;
            ((NotificationCpu)notification).TriggerType = this.TriggerType;
            ((NotificationCpu)notification).AlertPercent = this.AlertPercent;
            ((NotificationCpu)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        public override Gtk.Widget GetEditorWidget()
        {
            return new CpuEditorWidget();
        }

        public override void Start()
        {
            if (Running)
                return;

            if (log == null)
            {
                log = LogManager.GetLogger(String.Format("{0}:{1}", GetTypeName(), this.Name));
            }

            log.Info("Starting CPU monitoring");

            base.Start();

            if (cpuCounter == null && performanceCounterInitializer == null)
            {
                log.Debug("Setting up to initialize performance counter in background");

                performanceCounterInitializer = new BackgroundWorker();
                performanceCounterInitializer.DoWork += (sender, e) => {
                    log.Debug("Initializing performance counter in background");
                    PerformanceCounter counter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                    //first value is always 0%
                    counter.NextValue();
                    //Start using the performance counter
                    cpuCounter = counter;
                    log.Debug("Performance counter initialization done");
                };
                performanceCounterInitializer.WorkerSupportsCancellation = false;
                performanceCounterInitializer.RunWorkerAsync();
            }

            //Start timer
            uint cpuUpdateTimeout = 0;

            if (this.Configuration == ConfigurationEnum.Alert)
            {
                cpuUpdateTimeout = (uint)this.CheckPeriod * 60000;
            }
            else
            {
                cpuUpdateTimeout = 1000;
            }

            log.DebugFormat("Starting timer at {0} ms", cpuUpdateTimeout);
            RefreshTimer = GLib.Timeout.Add(cpuUpdateTimeout, new GLib.TimeoutHandler(CheckUsage));
            log.Debug("CPU monitoring started");
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.Info("Stopping CPU monitoring");

            //Stop timer
            GLib.Source.Remove(RefreshTimer);

            base.Stop();
            log.Debug("CPU monitoring stopped");
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        private bool CheckUsage()
        {
            if (cpuCounter != null)
            {
                int cpuUsage = Convert.ToInt32(cpuCounter.NextValue());

                log.DebugFormat("CPU usage {0}%", cpuUsage);

                if (this.Configuration == ConfigurationEnum.Alert)
                {
                    if (this.TriggerType == TriggerTypeEnum.More && cpuUsage > this.AlertPercent || 
                        this.TriggerType == TriggerTypeEnum.Less && cpuUsage < this.AlertPercent)
                    {
                        log.Info("Trigger notification");
                        OnTriggered(String.Format("CPU usage at {0}%", cpuUsage));
                    }
                }
                else if (this.Configuration == ConfigurationEnum.Monitor)
                {
                    RgbColor c1;
                    RgbColor c2;

                    if (Pattern == null)
                    {
                        c1 = RgbColor.FromRgb(0, 255, 0);
                        c2 = RgbColor.FromRgb(255, 0, 0);
                    }
                    else
                    {
                        if (Pattern.Animations.Count == 0)
                        {
                            c1 = RgbColor.FromRgb(0, 255, 0);
                            c2 = RgbColor.FromRgb(255, 0, 0);
                        }
                        else if (Pattern.Animations.Count == 1)
                        {
                            c1 = RgbColor.FromRgb(0, 0, 0);
                            c2 = Pattern.Animations[0].Color;
                        }
                        else
                        {
                            c1 = Pattern.Animations[0].Color;
                            c2 = Pattern.Animations[1].Color;
                        }
                    }


                    byte r = (byte)(c1.R + (c2.R - c1.R) / 100.0 * cpuUsage);
                    byte g = (byte)(c1.G + (c2.G - c1.G) / 100.0 * cpuUsage);
                    byte b = (byte)(c1.B + (c2.B - c1.B) / 100.0 * cpuUsage);

                    OnColorSend(r, g, b);
                }
            }

            return true;
        }

        public override void Dispose()
        {
            Stop();
            base.Dispose();
        }
    }
}

