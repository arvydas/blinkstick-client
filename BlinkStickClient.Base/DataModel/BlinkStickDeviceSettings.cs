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

        [JsonIgnore]
        public BlinkStick Led;

        [JsonIgnore]
        public Boolean Touched { get; set; }

        private Boolean[] LedBusy = new Boolean[64];
        private byte[] LedFrame = new byte[64 * 3];
        private Boolean NeedsLedUpdate = false;

        public BlinkStickDeviceSettings()
        {
            this.Touched = true;
            this.BrightnessLimit = 100;
            this.Running = false;
        }

        public BlinkStickDeviceSettings(BlinkStick led)
        {
            this.Led = led;
            this.Serial = led.Serial;
            this.Touched = true;
            this.BrightnessLimit = 100;
            this.Running = false;

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
			default:
				break;
			}

			this.BlinkStickDevice = led.BlinkStickDevice;
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
                LedFrame[index * 3 + 0] = g;
                LedFrame[index * 3 + 1] = r;
                LedFrame[index * 3 + 2] = b;

                NeedsLedUpdate = true;
            }

            if (!Running)
            {
                this.Start();
            }

            OnSendColor(channel, index, r, g, b);
        }

        public void SetColor(DeviceNotification notification, byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            lock (this)
            {
                //TODO: Copy first and last indexes to event
                for (int i = notification.LedFirstIndex; i <= notification.LedLastIndex; i++)
                {
                    LedFrame[i * 3] = g;
                    LedFrame[i * 3 + 1] = r;
                    LedFrame[i * 3 + 2] = b;
                    OnSendColor(0, (byte)i, r, g, b);
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

            if (notification is DeviceNotification)
            {
                index = ((DeviceNotification)notification).LedFirstIndex * 3;
            }

            return RgbColor.FromRgb(LedFrame[index + 1], LedFrame[index], LedFrame[index + 2]);
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

                LedFrame[0] = g;
                LedFrame[1] = r;
                LedFrame[2] = b;
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
                this.SetColor(0, i, 0, 0, 0);
            }

            for (byte i = 0; i < LedsB; i++)
            {
                this.SetColor(0, i, 0, 0, 0);
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

                    if (ev != null && ev.Notification is PatternNotification)
                    {
                        ev.Started = DateTime.Now;

                        PatternNotification notification = ev.Notification as PatternNotification;
                        foreach (Animation animation in notification.Pattern.Animations)
                        {
                            Animation copyAnimation = new Animation();
                            copyAnimation.Assign(animation);
                            ev.Animations.Add(copyAnimation);
                        }

                        ev.Animations[0].ReferenceColor = GetColor(ev.Notification);

                        AssignBusyLeds(ev, true);
                        eventsPlaying.Add(ev);
                    }

                    //Prepare next frame of data to send to LEDs
                    for (int ii = eventsPlaying.Count - 1; ii >= 0; ii--)
                    {
                        TriggeredEvent evnt = eventsPlaying[ii];

                        RgbColor color = evnt.Animations[evnt.AnimationIndex].GetColor(evnt.Started.Value, DateTime.Now, evnt.Animations[evnt.AnimationIndex].ReferenceColor);

                        PatternNotification notification = evnt.Notification as PatternNotification;

                        lock (this)
                        {
                            SetColor(notification, color.R, color.G, color.B);
                        }

                        if (evnt.Animations[evnt.AnimationIndex].AnimationFinished)
                        {
                            evnt.AnimationIndex += 1;
                            if (evnt.AnimationIndex == evnt.Animations.Count)
                            {
                                eventsPlaying.RemoveAt(ii);
                                AssignBusyLeds(evnt, false);
                            }
                            else
                            {
                                evnt.Animations[evnt.AnimationIndex].ReferenceColor = GetColor(evnt.Notification);
                                evnt.Started = DateTime.Now;
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
                            Led.SetColor(LedFrame[1], LedFrame[0], LedFrame[2]);
                        }
                        else
                        {
                            byte[] frame = new byte[this.LedsR * 3];

                            Array.Copy(LedFrame, 0, frame, 0, frame.Length); 

                            Led.SetColors(0, frame);
                        }
                    }

                    Thread.Sleep(40);
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
            PatternNotification notification = (PatternNotification)ev.Notification;
            for (int i = notification.LedFirstIndex; i <= notification.LedLastIndex; i++)
            {
                //If there is at least one LED currently in use, event needs to wait
                if (LedBusy[i])
                    return false;
            }

            return true;
        }

        void AssignBusyLeds(TriggeredEvent ev, Boolean busy)
        {
            PatternNotification notification = (PatternNotification)ev.Notification;
            for (int i = notification.LedFirstIndex; i <= notification.LedLastIndex; i++)
            {
                LedBusy[i] = busy;
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

