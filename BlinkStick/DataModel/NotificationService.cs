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

        public NotificationService()
        {
            log.Debug("Creating service...");
            log.Info("Service created");
        }

        public void Start()
        {
            log.Info("Starting notification monitoring...");
            foreach (Notification n in DataModel.Notifications)
            {
                n.Triggered += NotificationTriggered;
            }

            DataModel.Notifications.CollectionChanged += NotificationListChanged;

            log.Info("Started.");
        }

        public void Stop()
        {
            log.Info("Stopping monitor...");
            foreach (Notification n in DataModel.Notifications)
            {
                n.Triggered -= NotificationTriggered;
            }

            DataModel.Notifications.CollectionChanged -= NotificationListChanged;

            log.Debug("Stopping device playback");
            foreach (BlinkStickDeviceSettings settings in DataModel.Devices)
            {
                settings.Stop();
            }

            log.Info("Service stopped.");
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

            PatternNotification notification = sender as PatternNotification;

            TriggeredEvent ev = new TriggeredEvent(notification, e.Message);

            DataModel.TriggeredEvents.Add(ev);

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

            lock (settings.EventQueue)
            {
                settings.EventQueue.Enqueue(ev);
            }

            settings.PlayNextEvent();
        }
    }
}

