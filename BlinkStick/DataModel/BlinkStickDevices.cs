using System;
using System.Collections.Generic;
using BlinkStickDotNet;

namespace BlinkStickClient.DataModel
{
    public class BlinkStickDevices
    {
        public List<BlinkStickDeviceSettings> Devices = new List<BlinkStickDeviceSettings>();

        public BlinkStickDevices()
        {
        }

        public Boolean AddIfDoesNotExist(BlinkStickDotNet.BlinkStick led)
        {
            Boolean newRecord = true;

            foreach (BlinkStickDeviceSettings current in Devices)
            {
                if (current.Serial == led.Serial)
                {
                    current.Touched = true;
                    if (current.Led == null)
                    {
                        current.Led = led;
                        current.Led.OpenDevice();
                    }
                    newRecord = false;
                }
            }

            if (newRecord)
            {
                BlinkStickDeviceSettings settings = new BlinkStickDeviceSettings(led);
                settings.Touched = true;
                Devices.Add(settings);
                led.OpenDevice();
            }

            return newRecord;
        }

        public void Untouch()
        {
            foreach (BlinkStickDeviceSettings settings in Devices)
            {
                settings.Touched = false;
            }
        }

        public void ProcessUntouched()
        {
            foreach (BlinkStickDeviceSettings settings in Devices)
            {
                if (!settings.Touched)
                {
                    settings.Led = null;
                }
            }
        }
    }
}

