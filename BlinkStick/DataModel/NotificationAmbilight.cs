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
    }
}

