using System;
using System.Net;
using BlinkStickClient.Utils;
using System.Text.RegularExpressions;
using BlinkStickDotNet;

namespace BlinkStickClient.DataModel
{
    public class NotificationRemoteControl : CustomNotification
    {
        private RemoteControlServer server;

        public String ApiBindAddress { get; set; }
        public int ApiAccessPort { get; set; }

        private Regex SetColorRegex = new Regex(@"^\/api\/v1\/set_color\/([A-Za-z\-\.0-9]+)$");
        private Regex PatternRegex = new Regex(@"^\/api\/v1\/play_pattern\/([A-Za-z\-\.0-9]+)$");

        private enum RouteEnum
        {
            Unrecognized,
            SetColor,
            PlayPattern
        }

        public override string GetTypeName()
        {
            return "Remote Control";
        }

        public NotificationRemoteControl()
        {
            this.ApiBindAddress = "127.0.0.1";
            this.ApiAccessPort = 9000;
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationRemoteControl();
            }

            ((NotificationRemoteControl)notification).ApiAccessPort = this.ApiAccessPort;
            ((NotificationRemoteControl)notification).ApiBindAddress = this.ApiBindAddress;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return HttpListener.IsSupported;
        }

        public override bool IsUnique()
        {
            return true;
        }

        public override void Start()
        {
            if (Running)
                return;

            log.InfoFormat("Starting {0}", GetTypeName());

            base.Start();

            server = new RemoteControlServer(this.ApiBindAddress, this.ApiAccessPort);
            server.RequestReceived += (object sender, RequestReceivedEventArgs e) => {
                if (e.Context.Request.Url.AbsolutePath == "/api/v1/test")
                {
                    server.SendResponseJson(200, 
                        new InfoResponse() { name = "BlinkStick Client", version = ApplicationDataModel.ApplicationVersion, versionFull = ApplicationDataModel.ApplicationFullVersion }, 
                        e.Context.Response);
                    e.Handled = true;
                }

                RouteEnum route = RouteEnum.Unrecognized;

                Match m = SetColorRegex.Match(e.Context.Request.Url.AbsolutePath);

                if (m.Success)
                {
                    route = RouteEnum.SetColor;
                }
                else
                {
                    m = PatternRegex.Match(e.Context.Request.Url.AbsolutePath);

                    if (m.Success)
                    {
                        route = RouteEnum.PlayPattern;
                    }
                }

                BlinkStickDeviceSettings ledSettings = null;
                if (route != RouteEnum.Unrecognized)
                {
                    String serial = m.Groups[1].ToString();

                    ledSettings = DataModel.FindBySerial(serial);

                    if (ledSettings == null)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("BlinkStick with serial number {0} not found", serial)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                    else if (ledSettings.Led == null)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("BlinkStick with serial number {0} not connected", serial)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }

                switch (route) {
                    case RouteEnum.SetColor:
                        ProcessSetColorRequest(e, ledSettings);
                        break;
                    case RouteEnum.PlayPattern:
                        ProcessPlayPatternRequest(e, ledSettings);
                        break;
                    default:
                        break;
                }
            };
            server.Start();

            log.DebugFormat("{0} started", GetTypeName());
        }

        private void ProcessSetColorRequest(RequestReceivedEventArgs e, BlinkStickDeviceSettings ledSettings)
        {
            RgbColor color = RgbColor.Black();
            int channel = 0;
            int firstLed = 0;
            int lastLed = 0;

            for (int i = 0; i < e.Context.Request.QueryString.AllKeys.Length; i++)
            {
                string key = e.Context.Request.QueryString.AllKeys[i].ToLower();
                string value = e.Context.Request.QueryString.GetValues(i)[0];

                if (key == "channel")
                {
                    try
                    {
                        channel = Convert.ToInt32(value);
                        if (channel < 0 || channel > 2)
                            throw new Exception("not within range of 0..2");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid channel parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "firstled")
                {
                    try
                    {
                        firstLed = Convert.ToInt32(value);
                        if (firstLed < 0 || firstLed > 63)
                            throw new Exception("not within range of 0..63");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid ledStart parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "lastled")
                {
                    try
                    {
                        lastLed = Convert.ToInt32(value);
                        if (lastLed < 0 || lastLed > 63)
                            throw new Exception("not within range of 0..63");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid ledEnd parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "color")
                {
                    try
                    {
                        color = RgbColor.FromString(value);
                    }
                    catch
                    {
                        try
                        {
                            color = RgbColor.FromString("#" + value);
                        }
                        catch
                        {
                            server.SendResponseJson(422, new ErrorResponse() {
                                error = "Invalid color parameter"
                            }, e.Context.Response);
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }

            try
            {
                Pattern pattern = new Pattern();
                pattern.Animations.Add(new Animation());
                pattern.Animations[0].AnimationType = AnimationTypeEnum.SetColor;
                pattern.Animations[0].DelaySetColor = 0;
                pattern.Animations[0].Color = color;
                OnPatternSend(channel, (byte)firstLed, (byte)lastLed, ledSettings, pattern, 1);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to send color {0}", ex);
            }
            server.SendResponseJson(200, null, e.Context.Response);

            e.Handled = true;
        }

        private void ProcessPlayPatternRequest(RequestReceivedEventArgs e, BlinkStickDeviceSettings ledSettings)
        {
            Pattern pattern = new Pattern();
            int channel = 0;
            int ledStart = 0;
            int ledEnd = 0;

            for (int i = 0; i < e.Context.Request.QueryString.AllKeys.Length; i++)
            {
                string key = e.Context.Request.QueryString.AllKeys[i].ToLower();
                string value = e.Context.Request.QueryString.GetValues(i)[0];

                if (key == "channel")
                {
                    try
                    {
                        channel = Convert.ToInt32(value);
                        if (channel < 0 || channel > 2)
                            throw new Exception("not within range of 0..2");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid channel parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "firstled")
                {
                    try
                    {
                        ledStart = Convert.ToInt32(value);
                        if (ledStart < 0 || ledStart > 63)
                            throw new Exception("not within range of 0..63");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid ledStart parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "lastled")
                {
                    try
                    {
                        ledEnd = Convert.ToInt32(value);
                        if (ledEnd < 0 || ledEnd > 63)
                            throw new Exception("not within range of 0..63");
                    }
                    catch (Exception ex)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Invalid ledEnd parameter: {0}", ex.Message)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
                else if (key == "pattern")
                {
                    pattern = DataModel.FindPatternByName(value);

                    if (pattern == null)
                    {
                        server.SendResponseJson(422, new ErrorResponse() {
                            error = String.Format("Pattern {0} not found", value)
                        }, e.Context.Response);
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (pattern == null)
            {
                server.SendResponseJson(422, new ErrorResponse() {
                    error = "Missing pattern parameter"
                }, e.Context.Response);
                e.Handled = true;
                return;
            }

            try
            {
                OnPatternSend(channel, (byte)ledStart, (byte)ledEnd, ledSettings, pattern, 1);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to send color {0}", ex);
            }
            server.SendResponseJson(200, null, e.Context.Response);

            e.Handled = true;
        }

        public override void Stop()
        {
            if (!Running)
                return;
            log.InfoFormat("Stopping {0}", GetTypeName());

            server.Stop();
            server = null;

            base.Stop();
            log.DebugFormat("{0} stopped", GetTypeName());
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        #region Response classes
        class InfoResponse
        {
            public String name;
            public String version;
            public String versionFull;
        }

        class ErrorResponse
        {
            public String error;
        }
        #endregion
    }
}

