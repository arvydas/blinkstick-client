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

		[JsonIgnore]
		public Boolean AnimationFinished { get; private set; }

        [JsonIgnore]
        public RgbColor ReferenceColor;

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

		public RgbColor GetColor (DateTime start, DateTime current, RgbColor refColor = null)
		{
			double fraction = 0;
			double frame = (current - start).TotalMilliseconds;

			switch (this.AnimationType) {
				case AnimationTypeEnum.SetColor:
					if (frame >= this.DelaySetColor)
					{
						this.AnimationFinished = true;
						frame = this.DelaySetColor;
					}

					break;
				case AnimationTypeEnum.Blink:
					if (frame >= this.DurationBlink * this.RepeatBlink)
					{
						this.AnimationFinished = true;
						frame = this.DurationBlink * this.RepeatBlink - 1;
					}

					if (frame % this.DurationBlink < this.DurationBlink / 2) {
						return this.Color;
					} else {
						return RgbColor.Black();
					}
				case AnimationTypeEnum.Pulse:
					if (frame >= this.DurationPulse * this.RepeatPulse)
					{
						this.AnimationFinished = true;
						frame = this.DurationPulse * this.RepeatPulse;
					}

					fraction = frame % this.DurationPulse / this.DurationPulse;

					byte r;
					byte g;
					byte b;

					if (fraction <= 0.5) //Fade in
					{
						fraction = fraction * 2;

						r = (byte)Math.Ceiling(Color.R * fraction);
						g = (byte)Math.Ceiling(Color.G * fraction);
						b = (byte)Math.Ceiling(Color.B * fraction);
					}
					else //Fade out
					{
						fraction = 1 - (fraction - 0.5) * 2;

						r = (byte)Math.Floor(Color.R * fraction);
						g = (byte)Math.Floor(Color.G * fraction);
						b = (byte)Math.Floor(Color.B * fraction);
					}

					return RgbColor.FromRgb(r, g, b);
				case AnimationTypeEnum.Morph:
					if (refColor == null)
						throw new ArgumentNullException("refColor", "Reference color is required for Morph animation");

					if (frame >= this.DurationMorph)
					{
						this.AnimationFinished = true;
						frame = this.DurationMorph;
					}

					fraction = frame / this.DurationMorph;

					if (Color.R > refColor.R)
					{
						r = (byte)Math.Ceiling(refColor.R + (Color.R - refColor.R) * fraction);
					}
					else
					{
						r = (byte)Math.Floor(refColor.R + (Color.R - refColor.R) * fraction);
					}

					if (Color.G > refColor.G)
					{
						g = (byte)Math.Ceiling(refColor.G + (Color.G - refColor.G) * fraction);
					}
					else
					{
						g = (byte)Math.Floor(refColor.G + (Color.G - refColor.G) * fraction);
					}

					if (Color.B > refColor.B)
					{
						b = (byte)Math.Ceiling(refColor.B + (Color.B - refColor.B) * fraction);
					}
					else
					{
						b = (byte)Math.Floor(refColor.B + (Color.B - refColor.B) * fraction);
					}

					return RgbColor.FromRgb(r, g, b);
				default:
					break;
			}


			return this.Color;
		}
    }

    public enum AnimationTypeEnum {
        SetColor,
        Blink,
        Morph,
        Pulse
    }
}

