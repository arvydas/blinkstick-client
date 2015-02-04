using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationIfttt : PatternNotification
    {
        public override string GetTypeName()
        {
            return "IFTTT";
        }

        public NotificationIfttt()
        {
        }
    }
}

