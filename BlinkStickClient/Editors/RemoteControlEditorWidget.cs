using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using System.Diagnostics;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class RemoteControlEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationRemoteControl Notification;

        public RemoteControlEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.CustomNotification notification)
        {
            this.Notification = (NotificationRemoteControl)notification;

            entryBindAddress.Text = Notification.ApiBindAddress;
            spinbuttonPort.Value = Notification.ApiAccessPort;
        }

        public bool IsValid(Gtk.Window window)
        {
            return true;
        }

        public void UpdateNotification()
        {
            Notification.ApiBindAddress = entryBindAddress.Text;
            Notification.ApiAccessPort = spinbuttonPort.ValueAsInt;
        }


        protected void OnButtonFixPermissionsClicked (object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo("netsh");
            startInfo.Verb = "runas";
            startInfo.Arguments = String.Format("http add urlacl http://{0}:{1}/ user=Everyone", entryBindAddress.Text, spinbuttonPort.ValueAsInt);

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, Gtk.MessageType.Error);
            }
        }
        #endregion
    }
}

