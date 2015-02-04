using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationCpu : PatternNotification
    {
        public override string GetTypeName()
        {
            return "CPU";
        }

        public NotificationCpu()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationCpu();
            }

            return base.Copy(notification);
        }
    }
}

