using System;
using System.Text.RegularExpressions;
using BlinkStickClient.Bayeux;
using BlinkStickDotNet;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class NotificationBlinkStickDotCom : DeviceNotification
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

        public override CustomNotification Copy(CustomNotification notification)
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

            if (Client.ClientState == BayeuxClient.ClientStateEnum.Disconnected)
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
                    int channel = this.LedChannel;
                    int ledStart = this.LedFirstIndex;
                    int ledEnd = this.LedLastIndex;
                    int repeat = 1;
                    int duration = 0;

                    if (e.Data.ContainsKey("channel"))
                    {
                        try
                        {
                            channel = Convert.ToInt32((string)e.Data["channel"]);
                        }
                        catch (Exception ex)
                        {
                            log.WarnFormat("Failed to convert channel parameter {0}", ex);
                        }

                        if (channel < 0 || channel > 2)
                        {
                            log.Warn("Invalid channel parameter, defaulting to 0");
                            channel = 0;
                        }
                    }

                    if (e.Data.ContainsKey("firstLed"))
                    {
                        try
                        {
                            ledStart = Convert.ToInt32((string)e.Data["firstLed"]);
                        }
                        catch (Exception ex)
                        {
                            log.WarnFormat("Failed to convert firstLed parameter {0}", ex);
                        }

                        if (ledStart < 0 || ledStart > 63)
                        {
                            log.Warn("Invalid firstLed parameter, defaulting to 0");
                            ledStart = 0;
                        }
                    }

                    if (e.Data.ContainsKey("lastLed"))
                    {
                        try
                        {
                            ledEnd = Convert.ToInt32((string)e.Data["lastLed"]);
                        }
                        catch (Exception ex)
                        {
                            log.WarnFormat("Failed to convert lastLed parameter {0}", ex);
                        }

                        if (ledEnd < 0 || ledEnd > 63)
                        {
                            log.Warn("Invalid lastLed parameter, defaulting to 0");
                            ledEnd = 0;
                        }
                    }

                    if (e.Data.ContainsKey("repeat"))
                    {
                        if (e.Data["repeat"] == "loop")
                        {
                            repeat = -1;
                        }
                        else
                        {
                            try
                            {
                                repeat = Convert.ToInt32((string)e.Data["repeat"]);
                            }
                            catch (Exception ex)
                            {
                                log.WarnFormat("Failed to convert repeat parameter {0}", ex);
                            }
                        }
                    }

                    if (e.Data.ContainsKey("duration"))
                    {
                        try
                        {
                            duration = Convert.ToInt32((string)e.Data["duration"]);
                        }
                        catch (Exception ex)
                        {
                            log.WarnFormat("Failed to convert duration parameter {0}", ex);
                        }
                    }


                    if (e.Data.ContainsKey ("status") && (String)e.Data ["status"] == "off") {
                        log.InfoFormat("Blinkstick device {0} turned off", this.Name);

                        Pattern pattern = new Pattern();
                        Animation animation = new Animation();
                        pattern.Animations.Add(animation);
                        animation.Color = RgbColor.Black();
                        animation.DelaySetColor = 1;

                        OnPatternSend(channel, ledStart, ledEnd, this.Device, pattern, 1, 0);

                        //OnColorSend(channel, ledStart, ledEnd, 0, 0, 0, this.Device);
                    } 
                    else if (e.Data.ContainsKey("color"))
                    {
                        String color = (String)e.Data ["color"];

                        log.InfoFormat ("New color received for Blinkstick device {0} - {1}", color, this.Name);

                        Pattern pattern = new Pattern();
                        Animation animation = new Animation();
                        pattern.Animations.Add(animation);
                        animation.Color = RgbColor.FromString (color);
                        animation.DelaySetColor = 1;

                        OnPatternSend(channel, ledStart, ledEnd, this.Device, pattern, 1, 0);

                        //RgbColor c = RgbColor.FromString (color);
                        //OnColorSend(channel, ledStart, ledEnd, c.R, c.G, c.B, this.Device);
                    }
                    else if (e.Data.ContainsKey("pattern"))
                    {
                        String patternName = (String)e.Data ["pattern"];

                        Pattern pattern = this.DataModel.FindPatternByName(patternName);

                        if (pattern != null)
                        {
                            OnPatternSend(channel, ledStart, ledEnd, this.Device, pattern, repeat, duration);
                        }
                        else
                        {
                            log.ErrorFormat ("Pattern request received for Blinkstick device {0} - {1}, but pattern not found", patternName, this.Name);
                        }
                    }
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

                while (Client.ClientState != BayeuxClient.ClientStateEnum.Disconnected)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }

            base.Stop();
            log.DebugFormat("{0} monitoring stopped", GetTypeName());
        }

    }
}

