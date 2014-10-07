using System;

namespace BlinkStickClient.Classes
{
    public class Animation
    {
        public AnimationTypeEnum AnimationType { get; set; }

        public int DelaySetColor { get; set; }

        public int DurationBlink { get; set; }
        public int RepeatBlink { get; set; }

        public int DurationPulse { get; set; }
        public int RepeatPulse { get; set; }

        public int DurationMorph { get; set; }

        public Animation()
        {
            this.AnimationType = AnimationTypeEnum.SetColor;
            this.DelaySetColor = 1;
            this.DurationBlink = 1000;
            this.RepeatBlink = 1;
            this.DurationPulse = 1000;
            this.RepeatPulse = 1;
            this.DurationMorph = 1000;
        }
    }

    public enum AnimationTypeEnum {
        SetColor,
        Blink,
        Morph,
        Pulse
    }
}

