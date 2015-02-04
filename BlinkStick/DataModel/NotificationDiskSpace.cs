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

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationDiskSpace();
            }

            return base.Copy(notification);
        }
    }
}

