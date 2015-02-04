using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationDiskSpace : PatternNotification
    {
        public override string GetTypeName()
        {
            return "Disk Space";
        }

        public NotificationDiskSpace()
        {
        }
    }
}

