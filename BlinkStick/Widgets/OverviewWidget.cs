using System;
using Gtk;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class OverviewWidget : Gtk.Bin
    {
        BlinkStickDevices BlinkStickDeviceList = new BlinkStickDevices();

        ListStore store = new ListStore(typeof (BlinkStickDeviceSettings));

        ColorPaletteWidget colorPaletteWidget = new ColorPaletteWidget();

        private BlinkStickDeviceSettings _SelectedBlinkStick;

        private BlinkStickDeviceSettings SelectedBlinkStick
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
                }
            }
        }

        public OverviewWidget()
        {
            this.Build();

            /*
            Gdk.Pixbuf image = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.blinkstick.png");
            image = image.ScaleSimple(image.Width / 2, image.Height / 2, Gdk.InterpType.Nearest);
            imageBlinkStickPreview.Pixbuf = image;
            */

            hbox1.PackStart(colorPaletteWidget);

            colorPaletteWidget.ColorClicked += (object sender, ColorClickedEventArgs e) => {
                if (SelectedBlinkStick != null && SelectedBlinkStick.Led != null)
                {
                    BlinkStickDotNet.RgbColor color = BlinkStickDotNet.RgbColor.FromGdkColor(e.Color.Red, e.Color.Green, e.Color.Blue);
                    SelectedBlinkStick.Led.SetColor(color.R, color.G, color.B);
                }
            };
    
            CellRendererPixbuf blinkstickConnectedCell = new CellRendererPixbuf();
            comboboxDevices.PackStart(blinkstickConnectedCell, false);
            comboboxDevices.SetCellDataFunc(blinkstickConnectedCell, BlinkStickConnectedRenderer);

            CellRendererText blinkStickDeviceSettingsCell = new CellRendererText();
            comboboxDevices.PackStart(blinkStickDeviceSettingsCell, true);
            comboboxDevices.AddAttribute (blinkStickDeviceSettingsCell, "text", 0);
            comboboxDevices.SetCellDataFunc(blinkStickDeviceSettingsCell, BlinkStickDeviceSettingsClassRenderer);

            RefreshDevices();

            comboboxDevices.Model = store;

            UpdateUI();
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

        public void RefreshDevices()
        {
            BlinkStickDeviceList.Untouch();

            foreach (BlinkStickDotNet.BlinkStick led in BlinkStickDotNet.BlinkStick.FindAll())
            {
                BlinkStickDeviceList.AddIfDoesNotExist(led);
            }

            BlinkStickDeviceList.ProcessUntouched();

            int previousDeviceIndex = comboboxDevices.Active;

            store.Clear();

            foreach (BlinkStickDeviceSettings entity in BlinkStickDeviceList.Devices) {
                store.AppendValues(entity);
            }

            SelectedBlinkStick = null;

            if (store.IterNChildren() >= 1)
            {
                if (previousDeviceIndex == -1)
                {
                    comboboxDevices.Active = 0;
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

        private void UpdateUI()
        {
            buttonConfigure.Sensitive = comboboxDevices.Active != -1 && SelectedBlinkStick != null && SelectedBlinkStick.Led != null;
            buttonDelete.Sensitive = comboboxDevices.Active != -1 && SelectedBlinkStick != null && SelectedBlinkStick.Led == null;
            blinkstickinfowidget2.UpdateUI(SelectedBlinkStick);

            if (SelectedBlinkStick != null && SelectedBlinkStick.Led != null)
            {
                blinkstickemulatorwidget1.EmulatedDevice = SelectedBlinkStick.Led.BlinkStickDevice;
            }
            else
            {
                blinkstickemulatorwidget1.EmulatedDevice = BlinkStickDotNet.BlinkStickDeviceEnum.Unknown;
            }
        }

        protected void OnComboboxDevicesChanged (object sender, EventArgs e)
        {
            TreeIter iter;

            (sender as Gtk.ComboBox).GetActiveIter(out iter);
            SelectedBlinkStick = (BlinkStickDeviceSettings)((sender as Gtk.ComboBox).Model.GetValue(iter, 0));

            UpdateUI();
        }

        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            RefreshDevices();
        }

        protected void OnButtonDeleteClicked (object sender, EventArgs e)
        {
            BlinkStickDeviceList.Devices.Remove(SelectedBlinkStick);
            RefreshDevices();
        }

        protected void OnButtonConfigureClicked (object sender, EventArgs e)
        {
            ConfigureBlinkStickDialog dialog = new ConfigureBlinkStickDialog();
            dialog.DeviceSettings = SelectedBlinkStick;
            dialog.UpdateUI();
            dialog.Run();
            dialog.Destroy();
        }
    }
}

