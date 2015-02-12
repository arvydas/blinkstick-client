using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DiskSpaceEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationDiskSpace Notification;

        public DiskSpaceEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.Notification notification)
        {
            Notification = (notification as NotificationDiskSpace);

            spinbuttonSize.Value = Notification.DriveFreeSpaceLimit;
            spinbuttonCheckPeriod.Value = Notification.CheckPeriod;
            comboboxSizeType.Active = (int)Notification.Measurement;

            int index = 0;
            foreach (string drive in Notification.GetDrives())
            {
                comboboxDrive.AppendText(drive);

                if (drive == Notification.Drive)
                {
                    comboboxDrive.Active = index;
                }

                index++;
            }
        }

        public bool IsValid(Gtk.Window window)
        {
            if (this.comboboxDrive.Active < 0)
            {
                MessageBox.Show(window, "Please select drive", Gtk.MessageType.Error);
                return false;
            }

            return true;
        }

        public void UpdateNotification()
        {
            Notification.DriveFreeSpaceLimit = spinbuttonSize.ValueAsInt;
            Notification.CheckPeriod = spinbuttonCheckPeriod.ValueAsInt;
            Notification.Measurement = (MeasurementEnum)comboboxSizeType.Active;
            Notification.Drive = comboboxDrive.ActiveText;
        }


        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            if (comboboxDrive.ActiveText == null || comboboxDrive.ActiveText == "")
            {
                labelCurrentValue.Text = "Value: (Please select drive)";
                return;
            }

            try
            {
                labelCurrentValue.Text = String.Format("Value: {0} of free space", this.Notification.GetValue(comboboxDrive.ActiveText));
            }
            catch (Exception ex)
            {
                labelCurrentValue.Text = String.Format("Value: {0}", ex.Message);
            }
        }
        #endregion
    }
}

