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
    }
}

