using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationGmail : PatternNotification
    {
        public override string GetTypeName()
        {
            return "GMail";
        }

        public NotificationGmail()
        {
        }
    }
}

