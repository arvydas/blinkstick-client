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
    }
}

