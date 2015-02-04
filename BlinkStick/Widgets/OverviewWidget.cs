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

        public OverviewWidget()
        {
            this.Build();

            hbox1.PackStart(colorPaletteWidget);

            colorPaletteWidget.ColorClicked += (object sender, ColorClickedEventArgs e) => {
                if (deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null)
                {
                    BlinkStickDotNet.RgbColor color = BlinkStickDotNet.RgbColor.FromGdkColor(e.Color.Red, e.Color.Green, e.Color.Blue);
                    if (blinkstickemulatorwidget1.SelectedLed == -1)
                    {
                        deviceComboboxWidget.SelectedBlinkStick.SetColor(color.R, color.G, color.B);
                        blinkstickemulatorwidget1.SetColor(e.Color);
                    }
                    else
                    {
                        deviceComboboxWidget.SelectedBlinkStick.SetColor(0, (byte)blinkstickemulatorwidget1.SelectedLed, color.R, color.G, color.B);
                        blinkstickemulatorwidget1.SetColor((byte)blinkstickemulatorwidget1.SelectedLed, e.Color);
                    }
                }
            };

            deviceComboboxWidget.DeviceChanged += (object sender, EventArgs e) => {
                UpdateUI();
            };
    
            RefreshDevices();

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

            deviceComboboxWidget.LoadDevices(BlinkStickDeviceList);
        }

        private void UpdateUI()
        {
            buttonConfigure.Sensitive = deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null;
            buttonDelete.Sensitive = deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led == null;
            blinkstickinfowidget2.UpdateUI(deviceComboboxWidget.SelectedBlinkStick);

            if (deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null)
            {
                blinkstickemulatorwidget1.EmulatedDevice = deviceComboboxWidget.SelectedBlinkStick.Led.BlinkStickDevice;
            }
            else
            {
                blinkstickemulatorwidget1.EmulatedDevice = BlinkStickDotNet.BlinkStickDeviceEnum.Unknown;
            }
        }

        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            RefreshDevices();
        }

        protected void OnButtonDeleteClicked (object sender, EventArgs e)
        {
            BlinkStickDeviceList.Devices.Remove(deviceComboboxWidget.SelectedBlinkStick);
            deviceComboboxWidget.LoadDevices(BlinkStickDeviceList);
        }

        protected void OnButtonConfigureClicked (object sender, EventArgs e)
        {
            ConfigureBlinkStickDialog dialog = new ConfigureBlinkStickDialog();
            dialog.DeviceSettings = deviceComboboxWidget.SelectedBlinkStick;
            dialog.UpdateUI();
            dialog.Run();
            dialog.Destroy();
        }
    }
}

