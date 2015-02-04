using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationImap : PatternNotification
    {
        public override string GetTypeName()
        {
            return "IMAP";
        }

        public NotificationImap()
        {
        }
    }
}

