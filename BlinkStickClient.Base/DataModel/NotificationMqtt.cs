using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationMqtt : Notification
    {
        public override string GetTypeName()
        {
            return "MQTT";
        }

        public NotificationMqtt()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationMqtt();
            }

            return base.Copy(notification);
        }
    }
}

