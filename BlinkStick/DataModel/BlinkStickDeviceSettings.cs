using System;

namespace BlinkStickClient.DataModel
{
    public class BlinkStickDeviceSettings
    {
        public String Serial { get; set; }

        public int LedsR { get; set; }
        public int LedsG { get; set; }
        public int LedsB { get; set; }

        public int BrightnessLimit { get; set; }

        public BlinkStickDotNet.BlinkStick Led;

        public Boolean Touched { get; set; }

        public BlinkStickDeviceSettings()
        {
            this.Touched = true;
            this.BrightnessLimit = 100;
        }

        public BlinkStickDeviceSettings(BlinkStickDotNet.BlinkStick led)
        {
            this.Led = led;
            this.Serial = led.Serial;
            this.Touched = true;
            this.BrightnessLimit = 100;
        }

        public override string ToString()
        {
            if (Led != null && Led.InfoBlock1.Trim() != "")
            {
                return Led.InfoBlock1;
            }
            else
            {
                return this.Serial;
            }
        }

        public void SetColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            if (Led != null)
            {
                Led.SetColor(channel, index, r, g, b);
            }
        }

        public void SetColor(byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            if (Led != null)
            {
                Led.SetColor(r, g, b);
            }
        }
    }
}

