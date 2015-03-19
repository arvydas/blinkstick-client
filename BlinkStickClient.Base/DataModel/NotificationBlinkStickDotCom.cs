using System;
using System.Text.RegularExpressions;
using BlinkStick.Bayeux;
using BlinkStickDotNet;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class NotificationBlinkStickDotCom : Notification
    {
        [JsonIgnore]
        public static BayeuxClient _client;

        [JsonIgnore]
        public BayeuxClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new BayeuxClient(DataModel.CurrentApiAccessAddress);
                    _client.Disconnected += (object sender, EventArgs e) => {
                        _client.CloseThread();
                    };
                }

                return _client;
            }
        }

        private static int ClientRefCounter = 0;

        public string AccessCode { get; set; }

        public override string GetTypeName()
        {
            return "BlinkStick.com";
        }

        public NotificationBlinkStickDotCom()
        {

        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationBlinkStickDotCom();
            }

            ((NotificationBlinkStickDotCom)notification).AccessCode = this.AccessCode;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return true;
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0} monitoring", GetTypeName());

            Client.DataReceived += ClientDataReceived;

            ClientRefCounter++;

            if (!Client.Working)
            {
                Client.Connect();
            }

            Client.Subscribe ("/devices/" + this.AccessCode);

            base.Start();

            log.DebugFormat("{0} monitoring started", GetTypeName());
        }

        void ClientDataReceived (object sender, BayeuxClient.DataReceivedEventArgs e)
        {
            Regex r = new Regex ("/([^/]+)$");
            Match m = r.Match (e.Channel);
            if (m.Success) {
                if (m.Groups[1].Value == this.AccessCode)
                {
                    if (e.Data.ContainsKey ("status")) {
                        if ((String)e.Data ["status"] == "off") {
                            log.InfoFormat("Blinkstick device {0} turned off", this.Name);
                            OnColorSend(0, 0, 0);
                            return;
                        }
                    }

                    // Handle the message
                    String color = (String)e.Data ["color"];

                    log.InfoFormat ("New color received for Blinkstick device {0} - {1}", color, this.Name);
                    RgbColor c = RgbColor.FromString (color);

                    OnColorSend(c.R, c.G, c.B);
                }
            }
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0} monitoring", GetTypeName());

            Client.DataReceived -= ClientDataReceived;

            Client.Unsubscribe ("/devices/" + this.AccessCode);

            ClientRefCounter--;

            if (ClientRefCounter == 0)
            {
                Client.Disconnect();
            }

            base.Stop();
            log.DebugFormat("{0} monitoring stopped", GetTypeName());
        }

    }
}

