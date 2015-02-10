using System;
using BlinkStickClient.DataModel;
using Gtk;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceComboboxWidget : Gtk.Bin
    {
        #region Events
        public event EventHandler<EventArgs> DeviceChanged;

        protected void OnDeviceChanged()
        {
            if (DeviceChanged != null)
            {
                DeviceChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        private Boolean ignoreComboboxChange = false;

        private ApplicationDataModel DataModel;
        private ListStore store = new ListStore(typeof (BlinkStickDeviceSettings));

        private BlinkStickDeviceSettings _SelectedBlinkStick;

        public BlinkStickDeviceSettings SelectedBlinkStick
        {
            get
            {
                return _SelectedBlinkStick;
            }

            set 
            {
                if (_SelectedBlinkStick != value)
                {
                    _SelectedBlinkStick = value;
                    OnDeviceChanged();
                }
            }
        }

        public Boolean AutoSelectDevice { get; set; }

        public DeviceComboboxWidget()
        {
            this.Build();

            this.AutoSelectDevice = true;

            CellRendererPixbuf blinkstickConnectedCell = new CellRendererPixbuf();
            comboboxDevices.PackStart(blinkstickConnectedCell, false);
            comboboxDevices.SetCellDataFunc(blinkstickConnectedCell, BlinkStickConnectedRenderer);

            CellRendererText blinkStickDeviceSettingsCell = new CellRendererText();
            comboboxDevices.PackStart(blinkStickDeviceSettingsCell, true);
            comboboxDevices.SetCellDataFunc(blinkStickDeviceSettingsCell, BlinkStickDeviceSettingsClassRenderer);
        }

        public void LoadDevices(ApplicationDataModel dataModel)
        {
            this.DataModel = dataModel;

            int previousDeviceIndex = comboboxDevices.Active;

            ignoreComboboxChange = true;

            store.Clear();

            foreach (BlinkStickDeviceSettings entity in dataModel.Devices) {
                store.AppendValues(entity);
            }

            comboboxDevices.Model = store;

            ignoreComboboxChange = false;

            if (store.IterNChildren() >= 1)
            {
                if (previousDeviceIndex == -1)
                {
                    if (AutoSelectDevice)
                    {
                        comboboxDevices.Active = 0;
                    }
                }
                else 
                {
                    while (store.IterNChildren() <= previousDeviceIndex)
                    {
                        previousDeviceIndex--;
                    }

                    comboboxDevices.Active = previousDeviceIndex;
                }
            }
            else
            {
                comboboxDevices.Active = -1;
            }
        }

        public void SelectBySerial(String serial)
        {
            TreeIter iter;

            if (comboboxDevices.Model.GetIterFirst(out iter))
            {
                do
                {
                    BlinkStickDeviceSettings settings = comboboxDevices.Model.GetValue(iter, 0) as BlinkStickDeviceSettings;
                    if (settings.Serial == serial)
                    {
                        comboboxDevices.SetActiveIter(iter);
                        break;
                    }
                } while (comboboxDevices.Model.IterNext(ref iter));
            }
        }

        private void BlinkStickConnectedRenderer(CellLayout cell_layout, CellRenderer cell, TreeModel model, TreeIter iter)
        {    
            BlinkStickDeviceSettings myclass = model.GetValue(iter, 0) as BlinkStickDeviceSettings;
            if (myclass != null)
            {
                (cell as CellRendererPixbuf).StockId = myclass.Led == null ? "gtk-no" : "gtk-yes";
            }
        }

        private void BlinkStickDeviceSettingsClassRenderer(CellLayout cell_layout, CellRenderer cell, TreeModel model, TreeIter iter)
        {    
            BlinkStickDeviceSettings myclass = model.GetValue(iter, 0) as BlinkStickDeviceSettings;
            if (myclass != null)
            {
                (cell as CellRendererText).Text = myclass.ToString();
                cell.Xalign = 0;
            }
        }

        protected void OnComboboxDevicesChanged (object sender, EventArgs e)
        {
            if (ignoreComboboxChange)
                return;

            TreeIter iter;

            (sender as Gtk.ComboBox).GetActiveIter(out iter);
            SelectedBlinkStick = (BlinkStickDeviceSettings)((sender as Gtk.ComboBox).Model.GetValue(iter, 0));
        }
    }
}

