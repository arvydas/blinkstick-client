using System;

namespace BlinkStickClient.DataModel
{
    public abstract class Notification : IDisposable
    {
        public String Name;

        public String BlinkStickSerial { get; set; }

        public abstract String GetTypeName();

        public Notification()
        {
        }

        public virtual void Dispose()
        {
            //Empty, nothing to dispose in base class
        }

        public virtual Notification Copy(Notification notification = null)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.Name = this.Name;
            notification.BlinkStickSerial = this.BlinkStickSerial;

            return notification;
        }
    }
}

