using System;
using OpenPop.Pop3;

namespace BlinkStickClient.DataModel
{
    public class NotificationPop3 : EmailNotification
    {
        public override string GetTypeName()
        {
            return "POP3";
        }

        public NotificationPop3()
        {
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationPop3();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return true;
        }

        public override int GetValue(String username, String password, String server, int port, Boolean useSsl)
        {
            // The client disconnects from the server when being disposed
            using(Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(server, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // Get the number of messages in the inbox
                return client.GetMessageCount();
            }
        }
    }
}

