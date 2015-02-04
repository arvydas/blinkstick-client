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
    }
}

