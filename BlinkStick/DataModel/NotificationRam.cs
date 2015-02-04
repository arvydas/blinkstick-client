using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationRam : PatternNotification
    {
        public override string GetTypeName()
        {
            return "RAM";
        }

        public NotificationRam()
        {
        }
    }
}

