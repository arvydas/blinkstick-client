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

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBattery();
            }

            return base.Copy(notification);
        }
    }
}

