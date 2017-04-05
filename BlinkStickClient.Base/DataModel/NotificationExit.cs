using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlinkStickClient.DataModel
{
    public class NotificationExit : PatternNotification
    {
        public NotificationExit()
        {
        }

        public override string GetTypeName()
        {
            return "BlinkStick Client Exit";
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationExit();
            }

            //((NotificationStart)notification). = this.;

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return true;
        }

        public override bool RequiresMonitoring()
        {
            return false;
        }
    }
}

