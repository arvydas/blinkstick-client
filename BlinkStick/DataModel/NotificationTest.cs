using System;

namespace BlinkStickClient.DataModel
{
    public class NotificationTest : PatternNotification
    {
        public override string GetTypeName()
        {
            return "Test";
        }

        public NotificationTest()
        {
        }

        public override Notification Copy(Notification notification)
        {
            if (notification == null)
            {
                notification = new NotificationTest();
            }

            return base.Copy(notification);
        }

        public override bool IsSupported()
        {
            return true;
        }

        public override Gtk.Widget GetEditorWidget()
        {
            return new TestEditorWidget();
        }

        public void Trigger()
        {
            OnTriggered("Triggered with button");
        }
    }
}

