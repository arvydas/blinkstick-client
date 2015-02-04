using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationAmbilight : Notification
    {
        public override string GetTypeName()
        {
            return "Ambilight";
        }

        public NotificationAmbilight()
        {
        }
    
        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationAmbilight();
            }

            return base.Copy(notification);
        }
    }
}

