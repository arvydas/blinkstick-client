using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class GmailEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationGmail Notification;

        public GmailEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.CustomNotification notification)
        {
            Notification = (notification as NotificationGmail);

            entryEmail.Text = Notification.Email;
            entryPassword.Text = Notification.Password;
            spinbuttonCheckPeriod.Value = Notification.CheckPeriod;
        }

        public bool IsValid(Gtk.Window window)
        {
            if (this.entryEmail.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter Email", Gtk.MessageType.Error);
                return false;
            }

            if (this.entryPassword.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter password", Gtk.MessageType.Error);
                return false;
            }

            return true;
        }

        public void UpdateNotification()
        {
            Notification.Email = entryEmail.Text;
            Notification.Password = entryPassword.Text;
            Notification.CheckPeriod = spinbuttonCheckPeriod.ValueAsInt;
        }

        #endregion

        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            if (this.entryEmail.Text.Trim() == "")
            {
                labelCurrentValue.Text = "Value: (Email missing)";
                return;
            }

            if (this.entryPassword.Text.Trim() == "")
            {
                labelCurrentValue.Text = "Value: (Password missing)";
                return;
            }

            try
            {
                labelCurrentValue.Text = String.Format("Value: {0} unread", this.Notification.GetValue(this.entryEmail.Text, this.entryPassword.Text));
            }
            catch (Exception ex)
            {
                labelCurrentValue.Text = String.Format("Value: {0}", ex.Message);
            }
        }
    }
}

