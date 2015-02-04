using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationTest : Notification
    {
        public override string GetTypeName()
        {
            return "Test";
        }

        public NotificationTest()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationTest();
            }

            return base.Copy(notification);
        }
    }
}

