using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationRemoteControl : Notification
    {
        public override string GetTypeName()
        {
            return "Remote Control";
        }

        public NotificationRemoteControl()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationRemoteControl();
            }

            return base.Copy(notification);
        }
    }
}

