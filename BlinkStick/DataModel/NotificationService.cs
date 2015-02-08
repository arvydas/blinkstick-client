using System;
using System.ComponentModel;
using System.Threading;
using log4net;

namespace BlinkStickClient.DataModel
{
    public class NotificationService
    {
        protected static readonly ILog log = LogManager.GetLogger("NotificationService");  

        public ApplicationDataModel DataModel;

        private BackgroundWorker monitorWorker;

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

            DataModel.TriggeredEvents.Add(new TriggeredEvent(sender as Notification, e.Message));
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

                Thread.Sleep (100);
            }

            log.Info("Service thread stopped");
        }
    }
}

