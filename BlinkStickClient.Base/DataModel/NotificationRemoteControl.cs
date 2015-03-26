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

                    OnColorSend(c.R, c.G, c.B);

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

