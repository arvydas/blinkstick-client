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

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            (notification as PatternNotification).Pattern = this.Pattern;
            (notification as PatternNotification).LedIndex = this.LedIndex;

            return base.Copy(notification);
        }
    }
}

