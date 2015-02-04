using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationBlinkStickDotCom : Notification
    {
        public override string GetTypeName()
        {
            return "BlinkStick.com";
        }

        public NotificationBlinkStickDotCom()
        {

        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBlinkStickDotCom();
            }

            return base.Copy(notification);
        }
    }
}

