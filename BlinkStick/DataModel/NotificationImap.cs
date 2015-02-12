using System;
using System.ComponentModel;
using System.Security.Authentication;
using ImapX;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class NotificationImap : EmailNotification
    {
        public override string GetTypeName()
        {
            return "IMAP";
        }

        public NotificationImap()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationImap();
            }

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

        public override int GetValue(String username, String password, String server, int port, Boolean useSsl)
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

