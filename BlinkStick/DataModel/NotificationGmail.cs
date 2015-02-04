using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationGmail : PatternNotification
    {
        public override string GetTypeName()
        {
            return "GMail";
        }

        public NotificationGmail()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationGmail();
            }

            return base.Copy(notification);
        }
    }
}

