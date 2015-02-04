using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationImap : PatternNotification
    {
        public override string GetTypeName()
        {
            return "IMAP";
        }

        public NotificationImap()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationImap();
            }

            return base.Copy(notification);
        }
    }
}

