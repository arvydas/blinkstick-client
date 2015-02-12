using System;
using Gtk;
using log4net;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public abstract class Notification : IDisposable
    {
        private ILog _log;

        protected ILog log 
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(String.Format("{0}:{1}", GetTypeName(), this.Name));
                }

                return _log;
            }
        }

        #region Events
        public event EventHandler<TriggeredEventArgs> Triggered;

        protected void OnTriggered(String message)
        {
            if (Triggered != null)
            {
                Triggered(this, new TriggeredEventArgs(message));
            }
        }

        public event EventHandler<ColorSendEventArgs> ColorSend;

        protected void OnColorSend(byte r, byte g, byte b)
        {
            if (ColorSend != null)
            {
                ColorSend(this, new ColorSendEventArgs(r, g, b));
            }
        }
        #endregion

        [JsonIgnore]
        public Boolean Running { protected set; get; }

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

        public virtual void Start()
        {
            Running = true;
        }

        public virtual void Stop()
        {
            Running = false;
        }

        public virtual Boolean RequiresMonitoring()
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", GetTypeName(), Name);
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

    public class ColorSendEventArgs: EventArgs
    {
        public byte R;
        public byte G;
        public byte B;

        public ColorSendEventArgs(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}

