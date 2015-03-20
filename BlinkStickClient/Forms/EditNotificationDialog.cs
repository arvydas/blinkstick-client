using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using BlinkStickDotNet;
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

        public EditNotificationDialog(String title, Gtk.Window parent, ApplicationDataModel dataModel, Notification notification) 
            : base (title, parent, Gtk.DialogFlags.Modal, new object[0])
        {
            this.Build();

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
            Notification.BlinkStickSerial = deviceComboboxWidget.SelectedBlinkStick.Serial;
            Notification.LedFirstIndex = spinbuttonLedsFrom.ValueAsInt;
            Notification.LedLastIndex = spinbuttonLedsTo.ValueAsInt;

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
            deviceComboboxWidget.SelectBySerial(Notification.BlinkStickSerial);
            spinbuttonLedsFrom.Value = Notification.LedFirstIndex;
            spinbuttonLedsTo.Value = Notification.LedLastIndex;

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

            HSeparator hseparator;

            Type editorType = NotificationRegistry.FindEditorType(Notification.GetType());

            object editorWidgetObject = Activator.CreateInstance(editorType);

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
                    spinbuttonLedsTo.Sensitive = true;
                    spinbuttonLedsFrom.Sensitive = true;
                    break;
            }
        }
    }
}

