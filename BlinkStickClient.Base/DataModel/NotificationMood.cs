using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationMood : PatternNotification
    {
        public MoodlightSpeedEnum MoodlightSpeed;

        public MoodlightSpeedEnum AnimationSpeed;

        private uint RefreshTimer;

        private Random random;

        private uint SwitchPeriod 
        {
            get
            {
                switch (MoodlightSpeed)
                {
                    case MoodlightSpeedEnum.Fast:
                        return 10000;
                    case MoodlightSpeedEnum.Normal:
                        return 60000;
                    case MoodlightSpeedEnum.Slow:
                        return 600000;
                    default:
                        return 600000;
                }
            }
        }

        private int TransitionPeriod 
        {
            get
            {
                switch (AnimationSpeed)
                {
                    case MoodlightSpeedEnum.Fast:
                        return 500;
                    case MoodlightSpeedEnum.Normal:
                        return 2000;
                    case MoodlightSpeedEnum.Slow:
                        return 5000;
                    default:
                        return 5000;
                }
            }
        }

        public override string GetTypeName()
        {
            return "Moodlight";
        }

        public NotificationMood()
        {
            this.MoodlightSpeed = MoodlightSpeedEnum.Normal;
            this.AnimationSpeed = MoodlightSpeedEnum.Normal;
            this.PatterConfigurable = false;

            CreateTemporaryPattern();
        }

        private void CreateTemporaryPattern()
        {
            this.Pattern = new Pattern("Mood");
            Animation animation = new Animation();
            animation.AnimationType = AnimationTypeEnum.Morph;
            animation.DurationMorph = 1000;
            this.Pattern.Animations.Add(animation);
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationMood();
            }

            ((NotificationMood)notification).MoodlightSpeed = this.MoodlightSpeed;
            ((NotificationMood)notification).AnimationSpeed = this.AnimationSpeed;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return true;
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0}", GetTypeName());

            random = new Random();

            base.Start();

            log.DebugFormat("Starting timer at {0} ms", this.SwitchPeriod);
            RefreshTimer = GLib.Timeout.Add(this.SwitchPeriod, new GLib.TimeoutHandler(SwitchColor));

            SwitchColor();

            log.DebugFormat("{0} started", GetTypeName());
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0}", GetTypeName());

            //Stop timer
            GLib.Source.Remove(RefreshTimer);

            base.Stop();
            log.DebugFormat("{0} stopped", GetTypeName());
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        protected bool SwitchColor()
        {
            byte[] colors = new byte[3];
            random.NextBytes(colors);
            if (this.Pattern == null || this.Pattern.Animations.Count != 1)
            {
                CreateTemporaryPattern();
            }

            this.Pattern.Animations[0].DurationMorph = TransitionPeriod;
            this.Pattern.Animations[0].Color = BlinkStickDotNet.RgbColor.FromRgb(colors[0], colors[1], colors[2]);
            OnTriggered();

            return true;
        }
    }

    public enum MoodlightSpeedEnum
    {
        Slow,
        Normal,
        Fast
    }
}

