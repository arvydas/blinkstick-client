using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ApplicationEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationApplication Notification;

        public ApplicationEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.Notification notification)
        {
            this.Notification = (NotificationApplication)notification;

            entrySearchString.Text = this.Notification.SearchString;
        }

        public bool IsValid(Gtk.Window window)
        {
            if (this.entrySearchString.Text.Trim() == "")
            {
                MessageBox.Show(window, "Search string can not be empty!", Gtk.MessageType.Error);
                return false;
            }

            return true;
        }

        public void UpdateNotification()
        {
            this.Notification.SearchString = entrySearchString.Text;
        }

        #endregion
    }
}

