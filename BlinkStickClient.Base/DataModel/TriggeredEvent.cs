using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class TriggeredEvent
    {
        public DateTime TimeStamp;

        public CustomNotification Notification;

        public String Message;

        [JsonIgnore]
        public String LastDisplay;

        [JsonIgnore]
        public DateTime? Started;

        [JsonIgnore]
        public int AnimationIndex;

        [JsonIgnore]
        public List<Animation> Animations = new List<Animation>();

        [JsonIgnore]
        public CustomNotification NotificationSnapshot;

        [JsonIgnore]
        public byte LedFirst;

        [JsonIgnore]
        public byte LedLast;

        [JsonIgnore]
        public Pattern Pattern;

        [JsonIgnore]
        public BlinkStickDeviceSettings Device;

        public TriggeredEvent()
        {
        }

        public TriggeredEvent(CustomNotification notification, String message)
        {
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.NotificationSnapshot = notification.Copy();
            this.Message = message;
        }

        public TriggeredEvent(CustomNotification notification, byte ledFirst, byte ledLast, BlinkStickDeviceSettings device, Pattern pattern)
        {
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.NotificationSnapshot = notification.Copy();
            this.LedFirst = ledFirst;
            this.LedLast = ledLast;
            this.Device = device;
            this.Pattern = pattern;
        }
    }
}

