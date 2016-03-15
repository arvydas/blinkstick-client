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
        public DateTime? EventStarted;

        [JsonIgnore]
        public DateTime? AnimationStarted;

        [JsonIgnore]
        public int AnimationIndex;

        [JsonIgnore]
        public List<Animation> Animations = new List<Animation>();

        [JsonIgnore]
        public CustomNotification NotificationSnapshot;

        [JsonIgnore]
        public int FirstLed;

        [JsonIgnore]
        public int LastLed;

        [JsonIgnore]
        public Pattern Pattern;

        [JsonIgnore]
        public int Channel;

        [JsonIgnore]
        public int Repeat;

        [JsonIgnore]
        public int RepeatCount;

        [JsonIgnore]
        public int Duration;

        [JsonIgnore]
        public BlinkStickDeviceSettings Device;

        public TriggeredEvent()
        {
        }

        public TriggeredEvent(PatternNotification notification, String message)
        {
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.NotificationSnapshot = ((CustomNotification)notification).Copy();
            this.Message = message;

            this.FirstLed = notification.LedFirstIndex;
            this.LastLed = notification.LedLastIndex;
            this.Device = notification.Device;
            this.Pattern = notification.Pattern;
            this.Repeat = 1;
            this.Duration = 0;
        }

        public TriggeredEvent(CustomNotification notification, int channel, int firstLed, int lastLed, 
            BlinkStickDeviceSettings device, Pattern pattern, int repeat, int duration)
        {
            this.Channel = channel;
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.NotificationSnapshot = notification.Copy();
            this.FirstLed = firstLed;
            this.LastLed = lastLed;
            this.Device = device;
            this.Pattern = pattern;
            this.Repeat = repeat;
            this.Duration = duration;
        }
    }
}

