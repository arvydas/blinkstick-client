using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class MoodlightEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationMood Notification;

        public MoodlightEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.CustomNotification notification)
        {
            this.Notification = (NotificationMood)notification;

            comboboxTransitionSpeed.Active = (int)this.Notification.MoodlightSpeed;
        }

        public bool IsValid(Gtk.Window window)
        {
            return true;
        }

        public void UpdateNotification()
        {
            this.Notification.MoodlightSpeed = (MoodlightSpeedEnum)comboboxTransitionSpeed.Active;
        }
        #endregion

    }
}

