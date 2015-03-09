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
                n.ColorSend += NotificationColor;
            }

            DataModel.Notifications.CollectionChanged += NotificationListChanged;
            DataModel.Notifications.ItemUpdated += NotificationListItemUpdated;

            foreach (Notification n in DataModel.Notifications)
            {
                n.DataModel = DataModel;
                if (n.RequiresMonitoring() && n.Enabled)
                {
                    n.Start();
                }
            }

            log.Info("Started.");
        }

        void NotificationListItemUpdated (object sender, ItemUpdatedEventArgs e)
        {
            Notification notification = e.Item as Notification;

            if (notification.RequiresMonitoring())
            {
                if (notification.Enabled)
                {
                    if (notification.Running)
                    {
                        log.DebugFormat("Notification {0} restarting", notification.ToString());
                        notification.Stop();
                    }
                    else
                    {
                        log.DebugFormat("Notification {0} starting", notification.ToString());
                    }
                    notification.Start();
                }
                else if (notification.Running)
                {
                    log.DebugFormat("Notification {0} stopping", notification.ToString());
                    notification.Stop();
                }
            }
        }

        public void Stop()
        {
            log.Info("Stopping monitor...");
            foreach (Notification n in DataModel.Notifications)
            {
                n.Triggered -= NotificationTriggered;
            }

            DataModel.Notifications.CollectionChanged -= NotificationListChanged;
            DataModel.Notifications.ItemUpdated -= NotificationListItemUpdated;

            foreach (Notification n in DataModel.Notifications)
            {
                if (n.RequiresMonitoring() && n.Running)
                {
                    n.Stop();
                }
            }

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
                    n.ColorSend += NotificationColor;
                    n.DataModel = DataModel;

                    if (n.RequiresMonitoring() && n.Enabled)
                    {
                        n.Start();
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Notification n in e.OldItems)
                {
                    n.Triggered -= NotificationTriggered;
                    n.ColorSend -= NotificationColor;
                    n.DataModel = null;;

                    if (n.RequiresMonitoring() && n.Enabled)
                    {
                        n.Stop();
                    }
                }
            }
        }

        void NotificationColor (object sender, ColorSendEventArgs e)
        {
            Notification notification = sender as Notification;

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

            if (!settings.Playing)
            {
                settings.Led.SetColor(e.R, e.G, e.B);
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

