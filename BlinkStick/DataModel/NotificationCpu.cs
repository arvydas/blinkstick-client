using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationCpu : PatternNotification
    {
        public override string GetTypeName()
        {
            return "CPU";
        }

        public NotificationCpu()
        {
        }
    }
}

