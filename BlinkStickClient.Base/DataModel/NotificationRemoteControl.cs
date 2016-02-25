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
                        new InfoResponse() { name = "BlinkStick Client", version = ApplicationDataModel.ApplicationVersion }, 
                        e.Context.Response);
                    e.Handled = true;
                }

                Regex r = new Regex(@"^\/api\/v1\/color/([A-Za-z\-\.0-9]+)/([a-zA-z0-9]+)$");

                Match m = r.Match(e.Context.Request.Url.AbsolutePath);

                if (m.Success)
                {
                    BlinkStickDeviceSettings ledSettings = DataModel.FindBySerial(m.Groups[1].ToString());

                    if (ledSettings == null)
                    {
                        server.SendResponseJson(422, 
                            new ErrorResponse() { error = String.Format("BlinkStick with serial number {0} not found", m.Groups[1].ToString()) }, 
                            e.Context.Response);

                        e.Handled = true;
                        return;
                    } 
                    else if (ledSettings.Led == null)
                    {
                        server.SendResponseJson(422, 
                            new ErrorResponse() { error = String.Format("BlinkStick with serial number {0} not connected", m.Groups[1].ToString()) }, 
                            e.Context.Response);

                        e.Handled = true;
                        return;
                    }

                    RgbColor c;

                    try
                    {
                        c = RgbColor.FromString(m.Groups[2].ToString());
                    }
                    catch
                    {
                        try
                        {
                            c = RgbColor.FromString("#" + m.Groups[2].ToString());
                        }
                        catch
                        {
                            server.SendResponseJson(422, 
                                new ErrorResponse() { error = String.Format("Color {0} is invalid", m.Groups[2].ToString()) }, 
                                e.Context.Response);

                            e.Handled = true;
                            return;
                        }
                    }

                    int channel = 0;
                    int ledStart = 0;
                    int ledEnd = 0;

                    for (int i = 0; i < e.Context.Request.QueryString.AllKeys.Length; i++)
                    {
                        string key = e.Context.Request.QueryString.AllKeys[i];

                        if (key.ToLower() == "channel")
                        {
                            try
                            {
                                channel = Convert.ToInt32(e.Context.Request.QueryString.GetValues(i)[0]);

                                if (channel < 0 || channel > 2)
                                    throw new Exception("not within range of 0..2");
                            }
                            catch (Exception ex)
                            {
                                server.SendResponseJson(422, 
                                    new ErrorResponse() { error = String.Format("Invalid channel parameter: {0}", ex.Message) }, 
                                    e.Context.Response);

                                e.Handled = true;
                                return;
                            }
                        } 
                        else if (key.ToLower() == "ledstart")
                        {
                            try
                            {
                                ledStart = Convert.ToInt32(e.Context.Request.QueryString.GetValues(i)[0]);

                                if (ledStart < 0 || ledStart > 63)
                                    throw new Exception("not within range of 0..63");
                            }
                            catch (Exception ex)
                            {
                                server.SendResponseJson(422, 
                                    new ErrorResponse() { error = String.Format("Invalid ledStart parameter: {0}", ex.Message) }, 
                                    e.Context.Response);

                                e.Handled = true;
                                return;
                            }
                        } 
                        else if (key.ToLower() == "ledend")
                        {
                            try
                            {
                                ledEnd = Convert.ToInt32(e.Context.Request.QueryString.GetValues(i)[0]);

                                if (ledEnd < 0 || ledEnd > 63)
                                    throw new Exception("not within range of 0..63");
                            }
                            catch (Exception ex)
                            {
                                server.SendResponseJson(422, 
                                    new ErrorResponse() { error = String.Format("Invalid ledEnd parameter: {0}", ex.Message) }, 
                                    e.Context.Response);

                                e.Handled = true;
                                return;
                            }
                        } 
                    }

                    try
                    {
                        Pattern pattern = new Pattern();
                        pattern.Animations.Add(new Animation());
                        pattern.Animations[0].AnimationType = AnimationTypeEnum.SetColor;
                        pattern.Animations[0].DelaySetColor = 0;
                        pattern.Animations[0].Color = c;

                        OnPatternSend(channel, (byte)ledStart, (byte)ledEnd, ledSettings, pattern);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Failed to send color {0}", ex);
                    }

                    server.SendResponseJson(200, null, e.Context.Response);

                    e.Handled = true;

                }
            };
            server.Start();

            log.DebugFormat("{0} started", GetTypeName());
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
        }

        class ErrorResponse
        {
            public String error;
        }
        #endregion
    }
}

