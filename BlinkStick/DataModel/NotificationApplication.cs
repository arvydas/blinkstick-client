using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationApplication : PatternNotification
    {
        public override string GetTypeName()
        {
            return "Active Application";
        }

        public NotificationApplication()
        {
        }
    }
}

