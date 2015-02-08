using System;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationService
    {
        protected static readonly ILog log = LogManager.GetLogger("NotificationService");  

        public ApplicationDataModel DataModel;

        private BackgroundWorker monitorWorker;

        private Queue<TriggeredEvent> EventQueue = new Queue<TriggeredEvent>();

        public NotificationService()
        {
            log.Debug("Creating service...");
            log.Info("Service created");
        }

        public void Start()
        {
            if (monitorWorker != null)
            {
                log.Warn("Service already started.");
                return;            
            }

            log.Info("Starting service...");
            foreach (Notification n in DataModel.Notifications)
            {
                n.Triggered += NotificationTriggered;
            }

            DataModel.Notifications.CollectionChanged += NotificationListChanged;
            monitorWorker = new BackgroundWorker ();
            monitorWorker.DoWork += new DoWorkEventHandler (monitorWorker_DoWork);
            monitorWorker.WorkerSupportsCancellation = true;
            monitorWorker.RunWorkerAsync ();
            log.Info("Service started.");
        }

        void NotificationListChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (Notification n in e.NewItems)
                {
                    n.Triggered += NotificationTriggered;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Notification n in e.OldItems)
                {
                    n.Triggered -= NotificationTriggered;
                }
            }
        }

        private void NotificationTriggered (object sender, TriggeredEventArgs e)
        {
            log.InfoFormat("Notification [{0}] \"{1}\" triggered. Message: {2}", 
                (sender as Notification).GetTypeName(), 
                (sender as Notification).Name, 
                e.Message);

            TriggeredEvent ev = new TriggeredEvent(sender as Notification, e.Message);

            DataModel.TriggeredEvents.Add(ev);

            lock (EventQueue)
            {
                EventQueue.Enqueue(ev);
            }
        }

        public void Stop()
        {
            if (monitorWorker == null)
            {
                log.Warn("Service already stopped.");
                return;            
            }

            log.Info("Stopping service...");
            foreach (Notification n in DataModel.Notifications)
            {
                n.Triggered -= NotificationTriggered;
            }

            monitorWorker.CancelAsync();
            monitorWorker = null;
            log.Info("Service stopped.");
        }

        void monitorWorker_DoWork (object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            log.Info("Service thread started");

            while (!worker.CancellationPending) {
                if (EventQueue.Count > 0)
                {
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

                Thread.Sleep (100);
            }

            log.Info("Service thread stopped");
        }

        void PlayPattern(BackgroundWorker worker, PatternNotification notification)
        {
            if (notification.Pattern == null)
            {
                log.WarnFormat("({0}) Pattern is not assigned", notification.Name);
                return;
            }

            BlinkStickDeviceSettings settings = DataModel.FindBySerial(notification.BlinkStickSerial);

            if (settings == null)
            {
                log.WarnFormat("({0}) BlinkStick with serial {1} not known", notification.Name, notification.BlinkStickSerial);
                return;
            }

            if (settings.Led == null)
            {
                log.WarnFormat("({0}) BlinkStick with serial {1} is not connected", notification.Name, notification.BlinkStickSerial);
                return;
            }

            log.InfoFormat("({0}) Playing pattern -{1}-", notification.Name, notification.Pattern.Name);

            foreach (Animation animation in notification.Pattern.Animations)
            {
                if (worker.CancellationPending)
                    return;

                switch (animation.AnimationType) {
                    case AnimationTypeEnum.SetColor:
                        settings.Led.SetColor(animation.Color);
                        settings.Led.WaitThread(animation.DelaySetColor);
                        break;
                    case AnimationTypeEnum.Blink:
                        settings.Led.Blink(animation.Color, animation.RepeatBlink, animation.DurationBlink);
                        break;
                    case AnimationTypeEnum.Pulse:
                        settings.Led.Pulse(animation.Color, animation.RepeatPulse, animation.DurationPulse);
                        break;
                    case AnimationTypeEnum.Morph:
                        settings.Led.Morph(animation.Color, animation.DurationMorph);
                        break;
                }
            }

            log.InfoFormat("({0}) Pattern -{1}- playback complete", notification.Name, notification.Pattern.Name);
        }
    }
}

