using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using BlinkStickClient.Classes;
using BlinkStickDotNet;
using Gtk;

namespace BlinkStickClient
{
    public partial class EditNotificationDialog : Gtk.Dialog
    {
        public ApplicationSettings ApplicationSettings;

        private CustomNotification _Notification;
        public CustomNotification Notification { 
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

        private Gtk.Window ParentForm;

        IEditorInterface editorInterface;

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
        }

        public EditNotificationDialog(String title, Gtk.Window parent, ApplicationDataModel dataModel, CustomNotification notification, ApplicationSettings settings) 
            : base (title, parent, Gtk.DialogFlags.Modal, new object[0])
        {
            this.Build();

            this.Title = title;
            this.ApplicationSettings = settings;

            this.deviceComboboxWidget.DeviceChanged += OnDeviceComboboxWidgetDeviceChanged;

            ParentForm = parent;

            CellRendererText blinkStickDeviceSettingsCell = new CellRendererText();
            comboboxPattern.PackStart(blinkStickDeviceSettingsCell, true);
            comboboxPattern.SetCellDataFunc(blinkStickDeviceSettingsCell, PaternTitleRenderer);

            comboboxPattern.Model = store;
            deviceComboboxWidget.AutoSelectDevice = false;

            this.DataModel = dataModel;
            this._Notification = notification;

            RefreshDevices();
            ObjectToControls();
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

            if (Notification is DeviceNotification && deviceComboboxWidget.SelectedBlinkStick == null)
            {
                MessageBox.Show(this, "Please select BlinkStick device", Gtk.MessageType.Error);
                return;
            }

            if (Notification is PatternNotification && SelectedPattern == null)
            {
                MessageBox.Show(this, "Please select a pattern", Gtk.MessageType.Error);
                return;
            }

            if (spinbuttonLedsFrom.ValueAsInt > spinbuttonLedsTo.ValueAsInt)
            {
                Utils.MessageBox.Show(this, "First LED index can not be greater than last LED index", MessageType.Error);
                return;
            }

            if (editorInterface != null && !editorInterface.IsValid(this))
            {
                return;
            }

            ControlsToObject();

            this.Respond(Gtk.ResponseType.Ok);
        }

        private void ControlsToObject()
        {
            Notification.Enabled = checkbuttonEnabled.Active;
            Notification.Name = entryName.Text;
            if (Notification is DeviceNotification)
            {
                ((DeviceNotification)Notification).BlinkStickSerial = deviceComboboxWidget.SelectedBlinkStick.Serial;
                ((DeviceNotification)Notification).LedFirstIndex = spinbuttonLedsFrom.ValueAsInt;
                ((DeviceNotification)Notification).LedLastIndex = spinbuttonLedsTo.ValueAsInt;
            }

            if (Notification is PatternNotification)
            {
                ((PatternNotification)Notification).Pattern = SelectedPattern;
            }

            if (editorInterface != null)
            {
                editorInterface.UpdateNotification();
            }
        }

        private void ObjectToControls()
        {
            checkbuttonEnabled.Active = Notification.Enabled;
            entryName.Text = Notification.Name;

            if (Notification is PatternNotification)
            {
                LoadPatterns((Notification as PatternNotification).Pattern);
            }
            else
            {
                table2.Remove(labelPattern);
                table2.Remove(comboboxPattern);
                table2.Remove(buttonEditPatterns);

                if (Notification is DeviceNotification)
                {
                    table2.NRows -= 1;
                }
            }

            if (Notification is DeviceNotification)
            {
                if (((DeviceNotification)Notification).BlinkStickSerial == "" || ((DeviceNotification)Notification).BlinkStickSerial == null)
                {
                    if (this.DataModel.Devices.Count == 1)
                    {
                        deviceComboboxWidget.SelectBySerial(this.DataModel.Devices[0].Serial);
                    }
                }
                else
                {
                    deviceComboboxWidget.SelectBySerial(((DeviceNotification)Notification).BlinkStickSerial);
                }
                    
                spinbuttonLedsFrom.Value = ((DeviceNotification)Notification).LedFirstIndex;
                spinbuttonLedsTo.Value = ((DeviceNotification)Notification).LedLastIndex;
            }
            else
            {
                table2.Remove(labelBlinkStick);
                table2.Remove(deviceComboboxWidget);
                table2.Remove(labelLeds);
                table2.Remove(hboxLedConfiguration);
                table2.NRows -= 3;
            }

            HSeparator hseparator;

            Type editorType = NotificationRegistry.FindEditorType(Notification.GetType());

            object editorWidgetObject = null;

            if (editorType != null)
            {
                editorWidgetObject = Activator.CreateInstance(editorType);
            }

            if (editorWidgetObject != null && editorWidgetObject is Widget)
            {
                Widget editorWidget = (Widget)editorWidgetObject;

                hseparator = new HSeparator();
                vbox3.PackEnd(hseparator);
                hseparator.ShowAll();

                if (editorWidget is IEditorInterface)
                {
                    editorInterface = (editorWidget as IEditorInterface);
                    editorInterface.SetNotification(Notification);
                }

                vbox3.PackEnd(editorWidget, true, true, 0);

                editorWidget.SizeAllocated += (o, args) => {
                    int x, y, w, h, myw, myh;
                    ParentForm.GetPosition(out x, out y);
                    ParentForm.GetSize(out w, out h);

                    GetSize(out myw, out myh);

                    this.GdkWindow.Move(x + (w - myw) / 2, y + (h - myh) / 2);
                };

                editorWidget.ShowAll();
            }

            OnDeviceComboboxWidgetDeviceChanged(null, null);

            hseparator = new HSeparator();
            vbox3.PackEnd(hseparator);
            hseparator.ShowAll();

            deviceComboboxWidget.Sensitive = this.ApplicationSettings.AllowModeChange;
        }

        void LoadPatterns(Pattern selectedPattern)
        {
            store.Clear();

            foreach (Pattern pattern in DataModel.Patterns)
            {
                TreeIter iter = store.AppendValues(pattern);
                if (pattern == selectedPattern)
                {
                    comboboxPattern.SetActiveIter(iter);
                }
            }
        }

        protected void OnDeviceComboboxWidgetDeviceChanged(object sender, EventArgs e)
        {
            if (deviceComboboxWidget.SelectedBlinkStick == null)
            {
                spinbuttonLedsTo.Sensitive = false;
                spinbuttonLedsFrom.Sensitive = false;
                return;
            }

            switch (deviceComboboxWidget.SelectedBlinkStick.BlinkStickDevice)
            {
                case BlinkStickDeviceEnum.BlinkStick:
                    spinbuttonLedsTo.Value = 0;
                    spinbuttonLedsFrom.Value = 0;

                    spinbuttonLedsTo.Sensitive = false;
                    spinbuttonLedsFrom.Sensitive = false;
                    break;
                case BlinkStickDeviceEnum.BlinkStickPro:
                case BlinkStickDeviceEnum.BlinkStickSquare:
                case BlinkStickDeviceEnum.BlinkStickStrip:
                case BlinkStickDeviceEnum.BlinkStickNano:
                case BlinkStickDeviceEnum.BlinkStickFlex:
                    spinbuttonLedsTo.Sensitive = true;
                    spinbuttonLedsFrom.Sensitive = true;
                    break;
            }
        }

        protected void OnButtonEditPatternsClicked(object sender, EventArgs e)
        {
            PatternDialog.ShowForm(DataModel, SelectedPattern);
            Pattern selectedPattern = SelectedPattern;
            LoadPatterns(selectedPattern);
        }
    }
}

