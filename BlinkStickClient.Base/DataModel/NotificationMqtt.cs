using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationMqtt : CustomNotification
    {
        public override string GetTypeName()
        {
            return "MQTT";
        }

        public NotificationMqtt()
        {
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationMqtt();
            }

            return base.Copy(notification);
        }
    }
}

