﻿using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationIfttt : PatternNotification
    {
        public override string GetTypeName()
        {
            return "IFTTT";
        }

        public NotificationIfttt()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationIfttt();
            }

            return base.Copy(notification);
        }
    }
}

