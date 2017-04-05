using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlinkStickClient.DataModel
{
    public class NotificationStart : PatternNotification
    {
        public override string GetTypeName()
        {
            return "BlinkStick Client Start";
        }

        public NotificationStart()
        {
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationStart();
            }

            //((NotificationStart)notification). = this.;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return false;
        }

        public override bool RequiresMonitoring()
        {
            return false;
        }
    }
}

