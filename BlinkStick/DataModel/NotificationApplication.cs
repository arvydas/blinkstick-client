using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationApplication : PatternNotification
    {
        public override string GetTypeName()
        {
            return "Active Application";
        }

        public NotificationApplication()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationApplication();
            }

            return base.Copy(notification);
        }
    }
}

