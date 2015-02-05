using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using Gtk;

namespace BlinkStickClient
{
    public partial class EditNotificationDialog : Gtk.Dialog
    {
        private Notification _Notification;
        public Notification Notification { 
            get
            {
                return _Notification;
            }
            set
            {
                if (_Notification != value)
                {
                    _Notification = value;

                    if (_Notification != null)
                    {
                        ObjectToControls();
                    }
                }
            }
        }

        private ListStore store = new ListStore(typeof (Pattern));

        public ApplicationDataModel DataModel;

        public Pattern SelectedPattern
        {
            get
            {
                TreeIter iter;
                comboboxPattern.GetActiveIter(out iter);
                return (Pattern)(comboboxPattern.Model.GetValue(iter, 0));
            }
        }

        public EditNotificationDialog()
        {
            this.Build();

            CellRendererText blinkStickDeviceSettingsCell = new CellRendererText();
            comboboxPattern.PackStart(blinkStickDeviceSettingsCell, true);
            comboboxPattern.AddAttribute (blinkStickDeviceSettingsCell, "text", 0);
            comboboxPattern.SetCellDataFunc(blinkStickDeviceSettingsCell, PaternTitleRenderer);

            comboboxPattern.Model = store;

            deviceComboboxWidget.AutoSelectDevice = false;
        }

        private void PaternTitleRenderer(CellLayout cell_layout, CellRenderer cell, TreeModel model, TreeIter iter)
        {    
            Pattern pattern = model.GetValue(iter, 0) as Pattern;
            if (pattern != null)
            {
                (cell as CellRendererText).Text = pattern.ToString();
            }
        }

        public void RefreshDevices()
        {
            this.deviceComboboxWidget.LoadDevices(DataModel);
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
            if (this.entryName.Text == "")
            {
                MessageBox.Show(this, "Please enter name", Gtk.MessageType.Error);
                return;
            }

            if (deviceComboboxWidget.SelectedBlinkStick == null)
            {
                MessageBox.Show(this, "Please select BlinkStick device", Gtk.MessageType.Error);
                return;
            }

            if (Notification is PatternNotification && SelectedPattern == null)
            {
                MessageBox.Show(this, "Please select a pattern", Gtk.MessageType.Error);
                return;
            }

            ControlsToObject();

            this.Respond(Gtk.ResponseType.Ok);
        }

        private void ControlsToObject()
        {
            Notification.Name = entryName.Text;
            Notification.BlinkStickSerial = deviceComboboxWidget.SelectedBlinkStick.Serial;

            if (Notification is PatternNotification)
            {
                ((PatternNotification)Notification).Pattern = SelectedPattern;
            }

        }

        private void ObjectToControls()
        {
            entryName.Text = Notification.Name;
            deviceComboboxWidget.SelectBySerial(Notification.BlinkStickSerial);

            if (Notification is PatternNotification)
            {
                foreach (Pattern pattern in DataModel.Patterns)
                {
                    TreeIter iter = store.AppendValues(pattern);

                    if (pattern == (Notification as PatternNotification).Pattern)
                    {
                        comboboxPattern.SetActiveIter(iter);
                    }
                }
            }
            else
            {
                table2.Remove(labelPattern);
                table2.Remove(comboboxPattern);
                table2.Remove(buttonPlayPattern);
            }
        }
    }
}

