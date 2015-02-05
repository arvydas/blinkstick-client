using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationBoblight : Notification
    {
        public override string GetTypeName()
        {
            return "Boblight";
        }

        public NotificationBoblight()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBoblight();
            }

            return base.Copy(notification);
        }
    }
}

