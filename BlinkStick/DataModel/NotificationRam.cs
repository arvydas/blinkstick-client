using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationRam : PatternNotification
    {
        public override string GetTypeName()
        {
            return "RAM";
        }

        public NotificationRam()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationRam();
            }

            return base.Copy(notification);
        }
    }
}

