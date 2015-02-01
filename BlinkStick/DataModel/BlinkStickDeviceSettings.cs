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

        public BlinkStickDeviceSettings()
        {
        }

        public BlinkStickDeviceSettings(BlinkStickDotNet.BlinkStick led)
        {
            this.Led = led;
            this.Serial = led.Serial;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.Serial, Led == null ? " (Disconnected)" : "");
        }
    }
}

