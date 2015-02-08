using System;
using Gtk;

namespace BlinkStickClient.DataModel
{
    public abstract class Notification : IDisposable
    {
        #region Events
        public event EventHandler<TriggeredEventArgs> Triggered;

        protected void OnTriggered(String message)
        {
            if (Triggered != null)
            {
                Triggered(this, new TriggeredEventArgs(message));
            }
        }
        #endregion

        public Boolean Enabled;

        public String Name;

        public String BlinkStickSerial { get; set; }

        public abstract String GetTypeName();

        /// <summary>
        /// Gets the editor widget for the notification type.
        /// </summary>
        /// <returns>The editor widget object.</returns>
        public virtual Widget GetEditorWidget()
        {
            return null;
        }

        public Notification()
        {
            this.Enabled = true;
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

            notification.Enabled = this.Enabled;
            notification.Name = this.Name;
            notification.BlinkStickSerial = this.BlinkStickSerial;

            return notification;
        }

        /// <summary>
        /// Determines whether notification is supported for current running platform.
        /// All inherited notifications must override this method to check if they are supported for current running platform.
        /// </summary>
        /// <returns><c>true</c> if this instance is supported; otherwise, <c>false</c>.</returns>
        public virtual Boolean IsSupported()
        {
            return false;
        }
    }

    public class TriggeredEventArgs: EventArgs
    {
        public String Message;

        public TriggeredEventArgs(String message)
        {
            this.Message = message;
        }
    }
}

