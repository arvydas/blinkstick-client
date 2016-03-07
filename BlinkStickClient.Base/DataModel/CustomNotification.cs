using System;
using Gtk;
using log4net;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public abstract class CustomNotification : IDisposable
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
        public event EventHandler<ColorSendEventArgs> ColorSend;

        protected void OnColorSend(int channel, int firstLed, int lastLed, byte r, byte g, byte b, BlinkStickDeviceSettings settings)
        {
            if (ColorSend != null)
            {
                ColorSend(this, new ColorSendEventArgs(channel, firstLed, lastLed, r, g, b, settings));
            }
        }

        public event EventHandler<PatternSendEventArgs> PatternSend;

        protected void OnPatternSend(int channel, int firstLed, int lastLed, BlinkStickDeviceSettings device, Pattern pattern, int repeat)
        {
            if (PatternSend != null)
            {
                PatternSend(this, new PatternSendEventArgs(channel, firstLed, lastLed, device, pattern, repeat));
            }
        }
        #endregion

        [JsonIgnore]
        public Boolean Running { protected set; get; }

        public Boolean Enabled;

        public String Name;

        [JsonIgnore]
        public ApplicationDataModel DataModel;

        public abstract String GetTypeName();

        public CustomNotification()
        {
            this.Enabled = true;
        }

        public virtual void Dispose()
        {
            Stop();
        }

        public virtual CustomNotification Copy(CustomNotification notification = null)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.Enabled = this.Enabled;
            notification.Name = this.Name;

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

        /// <summary>
        /// Determines whether notification has to be unique.
        /// </summary>
        /// <returns><c>true</c> if this instance is unique; otherwise, <c>false</c>.</returns>
        public virtual Boolean IsUnique()
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
        public int Channel;
        public int FirstLed;
        public int LastLed;
        public BlinkStickDeviceSettings Device;

        public ColorSendEventArgs(int channel, int firstLed, int lastLed, byte r, byte g, byte b, BlinkStickDeviceSettings device)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.Channel = channel;
            this.FirstLed = firstLed;
            this.LastLed = lastLed;
            this.Device = device;
        }
    }

    public class PatternSendEventArgs: EventArgs
    {
        public int Channel;
        public int FirstLed;
        public int LastLed;
        public BlinkStickDeviceSettings Device;
        public Pattern Pattern;
        public int Repeat;

        public PatternSendEventArgs(int channel, int firstLed, int lastLed, BlinkStickDeviceSettings device, Pattern pattern, int repeat)
        {
            this.Channel = channel;
            this.FirstLed = firstLed;
            this.LastLed = lastLed;
            this.Device = device;
            this.Pattern = pattern;
            this.Repeat = repeat;
        }
    }
}

