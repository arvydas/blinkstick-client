using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationBoblight : CustomNotification
    {
        public override string GetTypeName()
        {
            return "Boblight";
        }

        public NotificationBoblight()
        {
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBoblight();
            }

            return base.Copy(notification);
        }
    }
}

