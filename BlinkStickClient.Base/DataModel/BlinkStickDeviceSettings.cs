using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using log4net;
using BlinkStickDotNet;

namespace BlinkStickClient.DataModel
{
    public class BlinkStickDeviceSettings
    {
        public event SendColorEventHandler SendColor;

        public Boolean OnSendColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (SendColor != null)
            {
                SendColor(this, new SendColorEventArgs(channel, index, r, g, b));
            }

            return true;
        }

        protected static readonly ILog log = LogManager.GetLogger("BlinkStickDeviceSettings");  

        [JsonIgnore]
        public Boolean Running { get; private set; }

        public String Serial { get; set; }

        public int LedsR { get; set; }
        public int LedsG { get; set; }
        public int LedsB { get; set; }

        public int BrightnessLimit { get; set; }

        private BackgroundWorker patternAnimator;

		public BlinkStickDeviceEnum BlinkStickDevice;

        [JsonIgnore]
        public Queue<TriggeredEvent> EventQueue = new Queue<TriggeredEvent>();

        private BlinkStick _Led;

        [JsonIgnore]
        public BlinkStick Led
        {
            get
            {
                return _Led;
            }
            set
            {
                if (_Led != value)
                {
                    _Led = value;

                    ApplyProperties();
                }
            }
        }

        [JsonIgnore]
        public Boolean Touched { get; set; }

        private Boolean[][] LedBusy = new Boolean[3][];
        private byte[][] LedFrame = new byte[3][];
        private Boolean NeedsLedUpdate = false;

        public BlinkStickDeviceSettings() : this(null)
        {
        }

        public BlinkStickDeviceSettings(BlinkStick led)
        {
            this.Led = led;

            this.Touched = true;
            this.BrightnessLimit = 100;
            this.Running = false;

            this.LedFrame[0] = new byte[64 * 3];
            this.LedFrame[1] = new byte[64 * 3];
            this.LedFrame[2] = new byte[64 * 3];

            this.LedBusy[0] = new Boolean[64];
            this.LedBusy[1] = new Boolean[64];
            this.LedBusy[2] = new Boolean[64];
        }

        private void ApplyProperties()
        {
            if (Led == null)
            {
                return;
            }

            this.Serial = Led.Serial;

            switch (this.Led.BlinkStickDevice) {
                case BlinkStickDeviceEnum.BlinkStick:
                    this.LedsR = 1;
                    this.LedsG = 1;
                    this.LedsB = 1;
                    break;
                case BlinkStickDeviceEnum.BlinkStickPro:
                    this.LedsR = 64;
                    this.LedsG = 64;
                    this.LedsB = 64;
                    break;
                case BlinkStickDeviceEnum.BlinkStickSquare:
                case BlinkStickDeviceEnum.BlinkStickStrip:
                    this.LedsR = 8;
                    this.LedsG = 0;
                    this.LedsB = 0;
                    break;
                case BlinkStickDeviceEnum.BlinkStickNano:
                    this.LedsR = 2;
                    this.LedsG = 0;
                    this.LedsB = 0;
                    break;
                case BlinkStickDeviceEnum.BlinkStickFlex:
                    this.LedsR = 32;
                    this.LedsG = 0;
                    this.LedsB = 0;
                    break;
                default:
                    break;
            }

            this.BlinkStickDevice = Led.BlinkStickDevice;
        }

        public override string ToString()
        {
            if (Led != null && Led.InfoBlock1.Trim() != "")
            {
                return Led.InfoBlock1;
            }
            else
            {
                return this.Serial;
            }
        }

        public void SetColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            lock (this)
            {
                LedFrame[channel][index * 3 + 0] = g;
                LedFrame[channel][index * 3 + 1] = r;
                LedFrame[channel][index * 3 + 2] = b;

                NeedsLedUpdate = true;
            }

            if (!Running)
            {
                this.Start();
            }

            OnSendColor(channel, index, r, g, b);
        }

        public void SetColor(int channel, int firstLed, int lastLed, byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            lock (this)
            {
                for (int i = firstLed; i <= lastLed; i++)
                {
                    LedFrame[channel][i * 3] = g;
                    LedFrame[channel][i * 3 + 1] = r;
                    LedFrame[channel][i * 3 + 2] = b;
                    OnSendColor((byte)channel, (byte)i, r, g, b);
                }

                NeedsLedUpdate = true;
            }

            if (!Running)
            {
                this.Start();
            }

        }

        public void SetColor(TriggeredEvent evnt, byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            lock (this)
            {
                for (int i = evnt.FirstLed; i <= evnt.LastLed; i++)
                {
                    LedFrame[evnt.Channel][i * 3] = g;
                    LedFrame[evnt.Channel][i * 3 + 1] = r;
                    LedFrame[evnt.Channel][i * 3 + 2] = b;
                    OnSendColor((byte)evnt.Channel, (byte)i, r, g, b);
                }

                NeedsLedUpdate = true;
            }


            if (!Running)
            {
                this.Start();
            }

        }

        public RgbColor GetColor(CustomNotification notification)
        {
            int index = 0;
            int channel = 0;

            if (notification is DeviceNotification)
            {
                index = ((DeviceNotification)notification).LedFirstIndex * 3;
                channel = ((DeviceNotification)notification).GetValidChannel();
            }

            return RgbColor.FromRgb(LedFrame[channel][index + 1], LedFrame[channel][index], LedFrame[channel][index + 2]);
        }

        public void SetColor(byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            lock (this)
            {
                NeedsLedUpdate = true;

                LedFrame[0][0] = g;
                LedFrame[0][1] = r;
                LedFrame[0][2] = b;
            }

            if (!Running)
            {
                this.Start();
            }

            OnSendColor(0, 0, r, g, b);
        }

        public void TurnOff()
        {
            for (byte i = 0; i < LedsR; i++)
            {
                this.SetColor(0, i, 0, 0, 0);
            }

            for (byte i = 0; i < LedsG; i++)
            {
                this.SetColor(1, i, 0, 0, 0);
            }

            for (byte i = 0; i < LedsB; i++)
            {
                this.SetColor(2, i, 0, 0, 0);
            }
        }

        public void Start()
        {
            if (Running)
                return;

            Running = true;

            patternAnimator = new BackgroundWorker ();
            patternAnimator.DoWork += new DoWorkEventHandler (patternAnimator_DoWork);
            patternAnimator.WorkerSupportsCancellation = true;
            patternAnimator.RunWorkerAsync ();
        }

        void patternAnimator_DoWork (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            try
            {
                log.InfoFormat("[{0}] Starting pattern playback", this.Serial);

                List<TriggeredEvent> eventsPlaying = new List<TriggeredEvent>();

                while (!worker.CancellationPending)
                {
                    TriggeredEvent ev = null;

                    lock (EventQueue)
                    {
                        for (int i = 0; i < EventQueue.Count; i++)
                        {
                            ev = EventQueue.Dequeue();

                            for (int j = eventsPlaying.Count - 1; j >= 0; j--)
                            {
                                if (ev.Notification == eventsPlaying[j].Notification && (eventsPlaying[j].Repeat < 0 || eventsPlaying[j].Duration > 0))
                                {
                                    log.DebugFormat("Removing infinite playback notifications as there is another pending");
                                    TriggeredEvent evPlaying = eventsPlaying[j];
                                    eventsPlaying.RemoveAt(j);
                                    AssignBusyLeds(evPlaying, false);
                                }
                            }

                            if (CanPlayEvent(ev))
                            {
                                break;
                            }
                            else
                            {
                                EventQueue.Enqueue(ev);
                                ev = null;
                            }
                        }
                    }

                    if (ev != null)
                    {
                        if (ev.NotificationSnapshot is PatternNotification)
                        {
                            ev.EventStarted = DateTime.Now;
                            ev.AnimationStarted = DateTime.Now;

                            PatternNotification notification = ev.NotificationSnapshot as PatternNotification;
                            foreach (Animation animation in notification.Pattern.Animations)
                            {
                                Animation copyAnimation = new Animation();
                                copyAnimation.Assign(animation);
                                ev.Animations.Add(copyAnimation);
                            }

                            ev.Animations[0].ReferenceColor = GetColor(ev.NotificationSnapshot);

                            AssignBusyLeds(ev, true);
                            eventsPlaying.Add(ev);
                        }
                        else
                        {
                            ev.AnimationStarted = DateTime.Now;
                            ev.EventStarted = DateTime.Now;

                            foreach (Animation animation in ev.Pattern.Animations)
                            {
                                Animation copyAnimation = new Animation();
                                copyAnimation.Assign(animation);
                                ev.Animations.Add(copyAnimation);
                            }

                            ev.Animations[0].ReferenceColor = GetColor(ev.NotificationSnapshot);

                            AssignBusyLeds(ev, true);
                            eventsPlaying.Add(ev);
                        }
                    } 

                    //Prepare next frame of data to send to LEDs
                    for (int ii = eventsPlaying.Count - 1; ii >= 0; ii--)
                    {
                        TriggeredEvent evnt = eventsPlaying[ii];

                        RgbColor color = evnt.Animations[evnt.AnimationIndex].GetColor(evnt.AnimationStarted.Value, DateTime.Now, evnt.Animations[evnt.AnimationIndex].ReferenceColor);

                        PatternNotification notification = evnt.NotificationSnapshot as PatternNotification;

                        lock (this)
                        {
                            SetColor(evnt, color.R, color.G, color.B);
                        }

                        if (evnt.Animations[evnt.AnimationIndex].AnimationFinished)
                        {
                            evnt.AnimationIndex += 1;

                            if (evnt.Duration > 0 && evnt.EventStarted.Value.AddMilliseconds(evnt.Duration) <= DateTime.Now)
                            {
                                eventsPlaying.RemoveAt(ii);
                                AssignBusyLeds(evnt, false);
                            }
                            else if (evnt.AnimationIndex == evnt.Animations.Count)
                            {
                                evnt.RepeatCount += 1;

                                if (evnt.Duration > 0 || evnt.Repeat < 0 || evnt.RepeatCount < evnt.Repeat)
                                {
                                    evnt.AnimationStarted = DateTime.Now;
                                    evnt.AnimationIndex = 0;

                                    evnt.Animations.ForEach( delegate(Animation a) { a.Reset(); });
                                }
                                else
                                {
                                    eventsPlaying.RemoveAt(ii);
                                    AssignBusyLeds(evnt, false);
                                }
                            }
                            else
                            {
                                evnt.Animations[evnt.AnimationIndex].ReferenceColor = GetColor(evnt.NotificationSnapshot);
                                evnt.AnimationStarted = DateTime.Now;
                            }
                        }
                    }

                    if (Led != null && NeedsLedUpdate)
                    {
                        lock (this)
                        {
                            NeedsLedUpdate = false;
                        }

                        if (Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStick || 
                            Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickPro && Led.Mode < 2)
                        {
                            Led.SetColor(LedFrame[0][1], LedFrame[0][0], LedFrame[0][2]);
                        }
                        else
                        {
                            byte[] frame = new byte[this.LedsR * 3];
                            Array.Copy(LedFrame[0], 0, frame, 0, frame.Length); 
                            Led.SetColors(0, frame);

                            int sleep = Math.Max(2, (int)(this.LedsR * 3 * 8f / 400f * 1.2)); //number of LEDs times 3 color elements times 8 bytes divided by speed
                            Thread.Sleep(sleep);

                            if (Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickPro)
                            {
                                frame = new byte[this.LedsG * 3];
                                Array.Copy(LedFrame[1], 0, frame, 0, frame.Length); 
                                Led.SetColors(1, frame);
                                sleep = Math.Max(2, (int)(this.LedsG * 3 * 8f / 400f * 1.2));
                                Thread.Sleep(sleep);

                                frame = new byte[this.LedsB * 3];
                                Array.Copy(LedFrame[2], 0, frame, 0, frame.Length); 
                                Led.SetColors(2, frame);

                                sleep = Math.Max(2, (int)(this.LedsB * 3 * 8f / 400f * 1.2));
                                Thread.Sleep(sleep);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Pattern playback crash {0}", ex);
            }

            log.InfoFormat("[{0}] Pattern playback stopped", this.Serial);

            Running = false;
        }

        Boolean CanPlayEvent(TriggeredEvent ev)
        {
            if (ev.NotificationSnapshot is PatternNotification)
            {
                PatternNotification notification = (PatternNotification)ev.NotificationSnapshot;
                int channel = notification.GetValidChannel();
                for (int i = notification.LedFirstIndex; i <= notification.LedLastIndex; i++)
                {
                    //If there is at least one LED currently in use, event needs to wait
                    if (LedBusy[channel][i])
                        return false;
                }
            }
            else
            {
                for (int i = ev.FirstLed; i <= ev.LastLed; i++)
                {
                    if (LedBusy[ev.Channel][i])
                        return false;
                }
            }

            return true;
        }

        void AssignBusyLeds(TriggeredEvent ev, Boolean busy)
        {
            if (ev.NotificationSnapshot is PatternNotification)
            {
                PatternNotification notification = (PatternNotification)ev.NotificationSnapshot;
                int channel = notification.GetValidChannel();
                for (int i = notification.LedFirstIndex; i <= notification.LedLastIndex; i++)
                {
                    LedBusy[channel][i] = busy;
                }
            }
            else
            {
                for (int i = ev.FirstLed; i <= ev.LastLed; i++)
                {
                    LedBusy[ev.Channel][i] = busy;
                }
            }
        }

        public void Stop()
        {
            if (patternAnimator != null && patternAnimator.IsBusy)
            {
                Led.Stop();
                patternAnimator.CancelAsync();
            }
        }
    }
}

