using System;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using BlinkStickDotNet;
using BlinkStickClient.Utils;
using Gdk;

namespace BlinkStickClient.DataModel
{
    public class Animation
    {
        public AnimationTypeEnum AnimationType { get; set; }

        [JsonProperty("Color")]
        public String ColorString {
            get {
                return String.Format("#{0:X2}{1:X2}{2:X2}", this.Color.R, this.Color.G, this.Color.B);
            }
            set {
                this.Color = RgbColor.FromString(value);
            }
        }

        [JsonIgnore]
        public RgbColor Color {
            get;
            set;
        }

        [JsonIgnore]
        public Color GtkColor {
            get {
                return new Gdk.Color(this.Color.R, this.Color.G, this.Color.B);
            }
            set {
                this.Color = value.ToRgbColor();
            }
        }

        public int DelaySetColor { get; set; }

        public int DurationBlink { get; set; }
        public int RepeatBlink { get; set; }

        public int DurationPulse { get; set; }
        public int RepeatPulse { get; set; }

        public int DurationMorph { get; set; }

        public Animation()
        {
            this.AnimationType = AnimationTypeEnum.SetColor;
            this.DelaySetColor = 1000;
            this.DurationBlink = 1000;
            this.RepeatBlink = 1;
            this.DurationPulse = 1000;
            this.RepeatPulse = 1;
            this.DurationMorph = 1000;
            this.ColorString = "#FFFFFF";
        }

        public void Assign(Animation animation)
        {
            this.AnimationType = animation.AnimationType;
            this.DelaySetColor = animation.DelaySetColor;
            this.DurationBlink = animation.DurationBlink;
            this.RepeatBlink = animation.RepeatBlink;
            this.DurationPulse = animation.DurationPulse;
            this.RepeatPulse = animation.RepeatPulse;
            this.DurationMorph = animation.DurationMorph;
            this.ColorString = animation.ColorString;
        }
    }

    public enum AnimationTypeEnum {
        SetColor,
        Blink,
        Morph,
        Pulse
    }
}

