using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    public partial class EditNotificationDialog : Gtk.Dialog
    {
        public Notification Notification { get; set; }

        public BlinkStickDevices BlinkStickDeviceList;

        public EditNotificationDialog()
        {
            this.Build();
        }

        public void RefreshDevices()
        {
            this.deviceComboboxWidget.LoadDevices(BlinkStickDeviceList);
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
            if (this.entryName.Text == "")
            {
                MessageBox.Show(this, "Please enter name", Gtk.MessageType.Error);
                return;
            }

            ControlsToObject();

            this.Respond(Gtk.ResponseType.Ok);
        }

        private void ControlsToObject()
        {
            Notification.Name = entryName.Text;
            Notification.BlinkStickSerial = deviceComboboxWidget.SelectedBlinkStick.Serial;
        }
    }
}

