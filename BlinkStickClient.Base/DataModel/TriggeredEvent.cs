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

        public TriggeredEvent()
        {
        }

        public TriggeredEvent(CustomNotification notification, String message)
        {
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.Message = message;
        }
    }
}

