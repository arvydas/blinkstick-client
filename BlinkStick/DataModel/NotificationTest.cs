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
    }
}

