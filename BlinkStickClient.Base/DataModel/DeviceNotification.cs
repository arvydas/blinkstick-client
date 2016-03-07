using System;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public abstract class DeviceNotification : CustomNotification
    {
        private String _BlinkStickSerial;
        public String BlinkStickSerial { 
            get
            {
                return _BlinkStickSerial;
            }
            set
            {
                if (_BlinkStickSerial != value)
                {
                    _BlinkStickSerial = value;
                    _Device = null;
                }
            }
        }

        public int LedFirstIndex { get; set; }
        public int LedLastIndex { get; set; }
        public int LedChannel { get; set; }

        private BlinkStickDeviceSettings _Device;

        [JsonIgnore]
        public BlinkStickDeviceSettings Device 
        {
            get 
            {
                if (_Device == null)
                {
                    _Device = DataModel.FindBySerial(this.BlinkStickSerial);
                }

                return _Device;
            }
        }

        public DeviceNotification()
        {
            this.LedFirstIndex = 0;
            this.LedLastIndex = 0;
            this.LedChannel = 0;
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
            ((DeviceNotification)notification).LedChannel = this.LedChannel;

            return notification;
        }

        public int GetValidChannel()
        {
            return this.LedChannel >= 0 && this.LedChannel <= 2 ? this.LedChannel : 0;
        }

        public void OnColorSend(int r, int g, int b)
        {
            OnColorSend(this.LedChannel, this.LedFirstIndex, this.LedLastIndex, (byte)r, (byte)g, (byte)b, this.Device);
        }
    }
}

