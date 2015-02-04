using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationBattery : PatternNotification
    {
        public override string GetTypeName()
        {
            return "Battery";
        }

        public NotificationBattery()
        {
        }
    }
}

