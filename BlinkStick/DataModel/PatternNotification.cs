using System;

namespace BlinkStickClient.DataModel
{
    public abstract class PatternNotification : Notification
    {
        public Pattern Pattern { get; set; }

        public Byte LedIndex { get; set; }

        public PatternNotification()
        {
        }
    }
}

