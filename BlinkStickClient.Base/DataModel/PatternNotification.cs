using System;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public abstract class PatternNotification : DeviceNotification
    {
        public Pattern Pattern { get; set; }

        public Byte LedIndex { get; set; }

        public Boolean PatterConfigurable { get; protected set; }

        public PatternNotification()
        {
            PatterConfigurable = true;
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            (notification as PatternNotification).Pattern = this.Pattern;
            (notification as PatternNotification).LedIndex = this.LedIndex;

            return base.Copy(notification);
        }
    }
}

