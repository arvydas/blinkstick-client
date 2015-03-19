using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationTest : PatternNotification
    {
        public TestFrequencyEnum Frequency { get; set; }

        private uint RefreshTimer;

        private uint TriggerPeriod {
            get
            {
                switch (Frequency)
                {
                    case TestFrequencyEnum.Disabled:
                        return 0;
                    case TestFrequencyEnum.S1:
                        return 1 * 1000;
                    case TestFrequencyEnum.S5:
                        return 5 * 1000;
                    case TestFrequencyEnum.S15:
                        return 15 * 1000;
                    case TestFrequencyEnum.S30:
                        return 30 * 1000;
                    case TestFrequencyEnum.M1:
                        return 1 * 60 * 1000;
                    case TestFrequencyEnum.M5:
                        return 5 * 60 * 1000;
                    case TestFrequencyEnum.M15:
                        return 15 * 60 * 1000;
                    case TestFrequencyEnum.M30:
                        return 30 * 60 * 1000;
                    case TestFrequencyEnum.H1:
                        return 60 * 60 * 1000;
                    default:
                        return 0;
                }
            }
        }

        public override string GetTypeName()
        {
            return "Test";
        }

        public NotificationTest()
        {
            this.Frequency = TestFrequencyEnum.Disabled;
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationTest();
            }

            ((NotificationTest)notification).Frequency = this.Frequency;

            return base.Copy(notification);
        }

        public override void Start()
        {
            if (Frequency == TestFrequencyEnum.Disabled || Running)
                return;

            log.InfoFormat("Starting {0} timer", GetTypeName());

            base.Start();

            log.DebugFormat("Starting timer at {0} ms", this.TriggerPeriod);
            RefreshTimer = GLib.Timeout.Add(this.TriggerPeriod, new GLib.TimeoutHandler(CheckUsage));

            log.DebugFormat("{0} timer started", GetTypeName());
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0} timer", GetTypeName());

            //Stop timer
            GLib.Source.Remove(RefreshTimer);

            base.Stop();
            log.DebugFormat("{0} timer stopped", GetTypeName());
        }

        protected bool CheckUsage()
        {
            OnTriggered();

            return true;
        }


        public override bool IsSupported()
        {
            return true;
        }

        public override bool RequiresMonitoring()
        {
            return Frequency != TestFrequencyEnum.Disabled;
        }

        public override Gtk.Widget GetEditorWidget()
        {
            return new TestEditorWidget();
        }

        public void Trigger()
        {
            OnTriggered("Triggered with button");
        }
    }

    public enum TestFrequencyEnum
    {
        Disabled,
        S1,
        S5,
        S15,
        S30,
        M1,
        M5,
        M15,
        M30,
        H1
    }
}

