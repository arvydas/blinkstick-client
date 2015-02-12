using System;
using System.Diagnostics;
using System.ComponentModel;
using BlinkStickDotNet;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlinkStickClient.DataModel
{
    public abstract class HardwareNotification : PatternNotification
    {
        public event EventHandler<EventArgs> Initialized;

        protected void OnInitialized()
        {
            if (Initialized != null)
            {
                Initialized(this, EventArgs.Empty);
            }
        }

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

        [JsonConverter(typeof(StringEnumConverter))]
        public TriggerTypeEnum TriggerType
        {
            get;
            set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationEnum Configuration { set; get; }

        public int AlertPercent { set; get; }

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        protected PerformanceCounter performanceCounter;

        private BackgroundWorker performanceCounterInitializer = null;

        [JsonIgnore]
        public Boolean IsInitialized { protected set; get; }

        public HardwareNotification()
        {
            this.TriggerType = TriggerTypeEnum.More;
            this.AlertPercent = 50;
            this.CheckPeriod = 1;
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationCpu();
            }

            ((HardwareNotification)notification).Configuration = this.Configuration;
            ((HardwareNotification)notification).TriggerType = this.TriggerType;
            ((HardwareNotification)notification).AlertPercent = this.AlertPercent;
            ((HardwareNotification)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0} monitoring", GetTypeName());

            base.Start();

            Initialize();

            //Start timer
            uint updateTimeout = 0;

            if (this.Configuration == ConfigurationEnum.Alert)
            {
                updateTimeout = (uint)this.CheckPeriod * 60000;
            }
            else
            {
                updateTimeout = 1000;
            }

            log.DebugFormat("Starting timer at {0} ms", updateTimeout);
            RefreshTimer = GLib.Timeout.Add(updateTimeout, new GLib.TimeoutHandler(CheckUsage));
            log.DebugFormat("{0} monitoring started", GetTypeName());
        }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                log.Debug("Setting up to initialize performance counter in background");

                performanceCounterInitializer = new BackgroundWorker();
                performanceCounterInitializer.DoWork += InitializePerformanceCounter;
                performanceCounterInitializer.WorkerSupportsCancellation = false;
                performanceCounterInitializer.RunWorkerAsync();
            }
        }

        protected virtual void InitializePerformanceCounter (object sender, DoWorkEventArgs e)
        {
            IsInitialized = true;
            OnInitialized();
        }

        public abstract int GetValue();

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
            if (IsInitialized)
            {
                int currentUsage = GetValue();

                log.DebugFormat("{0} usage {1}%", GetTypeName(), currentUsage);

                if (this.Configuration == ConfigurationEnum.Alert)
                {
                    if (this.TriggerType == TriggerTypeEnum.More && currentUsage > this.AlertPercent || 
                        this.TriggerType == TriggerTypeEnum.Less && currentUsage < this.AlertPercent)
                    {
                        log.Info("Trigger notification");
                        OnTriggered(String.Format("{0} usage at {1}%", GetTypeName(), currentUsage));
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


                    byte r = (byte)(c1.R + (c2.R - c1.R) / 100.0 * currentUsage);
                    byte g = (byte)(c1.G + (c2.G - c1.G) / 100.0 * currentUsage);
                    byte b = (byte)(c1.B + (c2.B - c1.B) / 100.0 * currentUsage);

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

