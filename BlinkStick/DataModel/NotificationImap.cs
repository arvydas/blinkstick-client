using System;
using System.ComponentModel;
using System.Security.Authentication;
using ImapX;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class NotificationImap : PatternNotification
    {
        public String Username { get; set; }

        public String Password { get; set; }

        public String ServerAddress { get; set; }

        public int Port { get; set; }

        public Boolean UseSsl;

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        public override string GetTypeName()
        {
            return "IMAP";
        }

        public NotificationImap()
        {
            UseSsl = true;
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationImap();
            }

            ((NotificationImap)notification).Username = this.Username;
            ((NotificationImap)notification).Password = this.Password;
            ((NotificationImap)notification).ServerAddress = this.ServerAddress;
            ((NotificationImap)notification).Port = this.Port;
            ((NotificationImap)notification).UseSsl = this.UseSsl;
            ((NotificationImap)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
        }

        public override Gtk.Widget GetEditorWidget()
        {
            return new EmailEditorWidget();
        }

        public override bool IsSupported()
        {
            return true;
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

        public int GetValue(String username, String password, String server, int port, Boolean useSsl)
        {
            var client = new ImapClient(server, port, useSsl ? SslProtocols.Default : SslProtocols.None, false);

            log.DebugFormat("Connecting to {0}:{1} SSL:{2}", server, port, useSsl);
            if (client.Connect())
            {
                log.DebugFormat("Logging in with {0}", username);

                if (client.Login(username, password))
                {
                    var messages = client.Folders.Inbox.Search("UNSEEN");

                    return messages.Length;
                }
                else
                {
                    throw new Exception("Log in error");
                }
            }
            else
            {
                throw new Exception("Connection error");
            }
        }
    }
}

