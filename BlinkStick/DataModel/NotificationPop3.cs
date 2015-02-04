using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationPop3 : PatternNotification
    {
        public override string GetTypeName()
        {
            return "POP3";
        }

        public NotificationPop3()
        {
        }
    }
}

