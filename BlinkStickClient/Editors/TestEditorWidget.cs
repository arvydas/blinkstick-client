using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class TestEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationTest Notification;

        public TestEditorWidget()
        {
            this.Build();
        }

        #region EditorInterface implementation
        public void SetNotification(BlinkStickClient.DataModel.Notification notification)
        {
            this.Notification = notification as NotificationTest;
            comboboxFrequency.Active = (int)Notification.Frequency;

            buttonTrigger.Sensitive = this.Notification.DataModel != null;
        }

        public bool IsValid(Gtk.Window window)
        {
            return true;
        }

        public void UpdateNotification()
        {
            Notification.Frequency = (TestFrequencyEnum)comboboxFrequency.Active;
        }
        #endregion

        protected void OnButtonTriggerClicked (object sender, EventArgs e)
        {
            this.Notification.Trigger();
        }
    }
}

