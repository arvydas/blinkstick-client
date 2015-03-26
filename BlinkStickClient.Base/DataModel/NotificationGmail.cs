using System;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace BlinkStickClient.DataModel
{
    public class NotificationGmail : PatternNotification
    {
        public String Email { get; set; }

        public String Password { get; set; }

        public int CheckPeriod { set; get; }

        private uint RefreshTimer;

        public override string GetTypeName()
        {
            return "GMail";
        }

        public NotificationGmail()
        {
            this.Email = "";
            this.Password = "";
            this.CheckPeriod = 1;
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationGmail();
            }

            ((NotificationGmail)notification).Email = this.Email;
            ((NotificationGmail)notification).Password = this.Password;
            ((NotificationGmail)notification).CheckPeriod = this.CheckPeriod;

            return base.Copy(notification);
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
            RefreshTimer = GLib.Timeout.Add((uint)(this.CheckPeriod * timeFrame), new GLib.TimeoutHandler(CheckUsage));

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

        protected bool CheckUsage()
        {
            int unreadEmails = 0;

            try
            {
                unreadEmails = GetValue(this.Email, this.Password);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return true;
            }

            log.DebugFormat("Unread emails: {0}", unreadEmails);

            if (unreadEmails > 0)
            {
                OnTriggered(String.Format("Unread emails: {0}", unreadEmails));
            }

            return true;
        }

        public int GetValue(String email, String password)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = Validator;
                WebRequest webGmailRequest = WebRequest.Create (@"https://mail.google.com/mail/feed/atom");
                webGmailRequest.PreAuthenticate = true;

                NetworkCredential loginCredentials = new NetworkCredential (email, password);
                webGmailRequest.Credentials = loginCredentials;

                WebResponse webGmailResponse = webGmailRequest.GetResponse ();
                Stream strmUnreadMailInfo = webGmailResponse.GetResponseStream ();

                StringBuilder sbUnreadMailInfo = new StringBuilder ();
                byte[] buffer = new byte[8192];
                int byteCount = 0;

                while ((byteCount = strmUnreadMailInfo.Read(buffer, 0, buffer.Length)) > 0)
                    sbUnreadMailInfo.Append (System.Text.Encoding.ASCII.GetString (buffer, 0, byteCount));

                XmlDocument UnreadMailXmlDoc = new XmlDocument ();
                UnreadMailXmlDoc.LoadXml (sbUnreadMailInfo.ToString ());
                XmlNodeList UnreadMailEntries = UnreadMailXmlDoc.GetElementsByTagName ("entry");

                return UnreadMailEntries.Count;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = e.Response as HttpWebResponse;

                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("Invalid username or password");
                    }
                    else
                    {
                        throw e;
                    }
                }
                else
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Validator (object sender, X509Certificate certificate, X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

