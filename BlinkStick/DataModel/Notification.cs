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
    }
}

