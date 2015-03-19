using System;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class TriggeredEvent
    {
        public DateTime TimeStamp;

        public Notification Notification;

        public String Message;

        [JsonIgnore]
        public String LastDisplay;

        [JsonIgnore]
        public DateTime? Started;

        [JsonIgnore]
        public int AnimationIndex;

        public TriggeredEvent()
        {
        }

        public TriggeredEvent(Notification notification, String message)
        {
            this.TimeStamp = DateTime.Now;
            this.Notification = notification;
            this.Message = message;
        }
    }
}

