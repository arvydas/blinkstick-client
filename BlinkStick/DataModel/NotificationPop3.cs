using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationPop3 : PatternNotification
    {
        public override string GetTypeName()
        {
            return "POP3";
        }

        public NotificationPop3()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationPop3();
            }

            return base.Copy(notification);
        }
    }
}

