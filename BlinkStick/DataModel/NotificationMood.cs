using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationMood : Notification
    {
        public override string GetTypeName()
        {
            return "Moodlight";
        }

        public NotificationMood()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationMood();
            }

            return base.Copy(notification);
        }
    }
}

