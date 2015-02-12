using System;
using System.ComponentModel;

namespace BlinkStickClient.DataModel
{
    public abstract class EmailNotification : PatternNotification
    {
        public String Username { get; set; }

        public String Password { get; set; }

        public String ServerAddress { get; set; }

        public int Port { get; set; }

        public Boolean UseSsl;

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        public EmailNotification()
        {
            UseSsl = true;
        }

        public override Notification Copy(Notification notification)
        {
            ((EmailNotification)notification).Username = this.Username;
            ((EmailNotification)notification).Password = this.Password;
            ((EmailNotification)notification).ServerAddress = this.ServerAddress;
            ((EmailNotification)notification).Port = this.Port;
            ((EmailNotification)notification).UseSsl = this.UseSsl;
            ((EmailNotification)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0} monitoring", GetTypeName());

            base.Start();

            #if DEBUG
            //Faster checks for debugging
            int timeFrame = 10000;
            #else
            int timeFrame = 60000;
            #endif
            log.DebugFormat("Starting timer at {0} ms", (uint)(this.CheckPeriod * timeFrame));
            RefreshTimer = GLib.Timeout.Add((uint)(this.CheckPeriod * timeFrame), new GLib.TimeoutHandler(CheckUnreadEmails));

            log.DebugFormat("{0} monitoring started", GetTypeName());
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0} monitoring", GetTypeName());

            //Stop timer
            GLib.Source.Remove(RefreshTimer);

            base.Stop();
            log.DebugFormat("{0} monitoring stopped", GetTypeName());
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        protected bool CheckUnreadEmails()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs e) => {
                int unreadEmails = 0;

                try
                {
                    unreadEmails = GetValue(this.Username, this.Password, this.ServerAddress, this.Port, this.UseSsl);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    return;
                }

                log.DebugFormat("Unread emails: {0}", unreadEmails);

                if (unreadEmails > 0)
                {
                    OnTriggered(String.Format("Unread emails: {0}", unreadEmails));
                }
            };
            worker.RunWorkerAsync();

            return true;
        }

        public abstract int GetValue(String username, String password, String server, int port, Boolean useSsl);
    }
}

