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

        public void AddIfDoesNotExist(BlinkStickDotNet.BlinkStick led)
        {
            foreach (BlinkStickDeviceSettings current in Devices)
            {
                if (current.Serial == led.Serial)
                    return;
            }

            BlinkStickDeviceSettings settings = new BlinkStickDeviceSettings(led);
            Devices.Add(settings);
        }
    }
}

