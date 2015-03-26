using System;

namespace BlinkStickClient.DataModel
{
    public abstract class DeviceNotification : CustomNotification
    {
        public String BlinkStickSerial { get; set; }

        public int LedFirstIndex { get; set; }
        public int LedLastIndex { get; set; }

        public DeviceNotification()
        {
            this.LedFirstIndex = 0;
            this.LedLastIndex = 0;
        }

        public override CustomNotification Copy(CustomNotification notification = null)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            ((DeviceNotification)notification).BlinkStickSerial = this.BlinkStickSerial;
            ((DeviceNotification)notification).LedFirstIndex = this.LedFirstIndex;
            ((DeviceNotification)notification).LedLastIndex = this.LedLastIndex;

            return notification;
        }
    }
}

