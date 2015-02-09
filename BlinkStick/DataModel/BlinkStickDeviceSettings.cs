using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class BlinkStickDeviceSettings
    {
        protected static readonly ILog log = LogManager.GetLogger("BlinkStickDeviceSettings");  

        [JsonIgnore]
        public Boolean Playing { get; private set; }

        public String Serial { get; set; }

        public int LedsR { get; set; }
        public int LedsG { get; set; }
        public int LedsB { get; set; }

        public int BrightnessLimit { get; set; }

        private BackgroundWorker patternPlayer;

        [JsonIgnore]
        public Queue<TriggeredEvent> EventQueue = new Queue<TriggeredEvent>();

        [JsonIgnore]
        public BlinkStickDotNet.BlinkStick Led;

        [JsonIgnore]
        public Boolean Touched { get; set; }

        public BlinkStickDeviceSettings()
        {
            this.Touched = true;
            this.BrightnessLimit = 100;
            this.Playing = false;
        }

        public BlinkStickDeviceSettings(BlinkStickDotNet.BlinkStick led)
        {
            this.Led = led;
            this.Serial = led.Serial;
            this.Touched = true;
            this.BrightnessLimit = 100;
            this.Playing = false;
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

            if (Led != null)
            {
                Led.SetColor(channel, index, r, g, b);
            }
        }

        public void SetColor(byte r, byte g, byte b)
        {
            if (BrightnessLimit < 100 && BrightnessLimit >= 0)
            {
                r = (byte)(BrightnessLimit / 100.0 * r);
                g = (byte)(BrightnessLimit / 100.0 * g);
                b = (byte)(BrightnessLimit / 100.0 * b);
            }

            if (Led != null)
            {
                Led.SetColor(r, g, b);
            }
        }

        public void PlayNextEvent()
        {
            if (EventQueue.Count == 0 || Playing)
                return;

            Playing = true;

            patternPlayer = new BackgroundWorker ();
            patternPlayer.DoWork += new DoWorkEventHandler (patternPlayer_DoWork);
            patternPlayer.WorkerSupportsCancellation = true;
            patternPlayer.RunWorkerAsync ();
        }

        void patternPlayer_DoWork (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            log.InfoFormat("[{0}] Starting pattern playback", this.Serial);

            while (EventQueue.Count > 0) {
                TriggeredEvent ev;

                lock (EventQueue)
                {
                    ev = EventQueue.Dequeue();
                }

                if (ev.Notification is PatternNotification)
                {
                    PlayPattern(worker, ev.Notification as PatternNotification);
                }
            }

            log.InfoFormat("[{0}] Pattern playback stopped", this.Serial);

            Playing = false;
        }

        void PlayPattern(BackgroundWorker worker, PatternNotification notification)
        {
            log.InfoFormat("({0}) Playing pattern -{1}-", notification.Name, notification.Pattern.Name);

            Led.Enable();

            foreach (Animation animation in notification.Pattern.Animations)
            {
                if (worker.CancellationPending)
                    return;

                switch (animation.AnimationType) {
                    case AnimationTypeEnum.SetColor:
                        Led.SetColor(animation.Color);
                        Led.WaitThread(animation.DelaySetColor);
                        break;
                    case AnimationTypeEnum.Blink:
                        Led.Blink(animation.Color, animation.RepeatBlink, animation.DurationBlink);
                        break;
                    case AnimationTypeEnum.Pulse:
                        Led.Pulse(animation.Color, animation.RepeatPulse, animation.DurationPulse);
                        break;
                    case AnimationTypeEnum.Morph:
                        Led.Morph(animation.Color, animation.DurationMorph);
                        break;
                }
            }

            log.InfoFormat("({0}) Pattern -{1}- playback complete", notification.Name, notification.Pattern.Name);
        }

        public void Stop()
        {
            if (patternPlayer.IsBusy)
            {
                Led.Stop();
                patternPlayer.CancelAsync();
            }
        }
    }
}

