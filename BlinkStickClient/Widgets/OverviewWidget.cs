using System;
using Gtk;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class OverviewWidget : Gtk.Bin
    {
        public ApplicationDataModel DataModel;

        ListStore store = new ListStore(typeof (BlinkStickDeviceSettings));

        ColorPaletteWidget colorPaletteWidget = new ColorPaletteWidget();

        private BlinkStickDeviceSettings PreviousDeviceSettings;

        public OverviewWidget()
        {
            this.Build();

            hbox1.PackStart(colorPaletteWidget);

            colorPaletteWidget.ColorClicked += (object sender, ColorClickedEventArgs e) => {
                if (deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null)
                {
                    RgbColor color = RgbColor.FromGdkColor(e.Color.Red, e.Color.Green, e.Color.Blue);
                    if (blinkstickemulatorwidget1.SelectedLed == -1)
                    {
                        deviceComboboxWidget.SelectedBlinkStick.SetColor(color.R, color.G, color.B);
                        //blinkstickemulatorwidget1.SetColor(e.Color);
                    }
                    else
                    {
                        deviceComboboxWidget.SelectedBlinkStick.SetColor(0, 
                            (byte)blinkstickemulatorwidget1.SelectedLed, color.R, color.G, color.B);
                        //blinkstickemulatorwidget1.SetColor((byte)blinkstickemulatorwidget1.SelectedLed, e.Color);
                    }
                }
            };

            deviceComboboxWidget.DeviceChanged += (object sender, EventArgs e) => {
                if (PreviousDeviceSettings != null && PreviousDeviceSettings.Led != null)
                {
                    PreviousDeviceSettings.SendColor -= BlinkStickSendColor;
                }

                PreviousDeviceSettings = deviceComboboxWidget.SelectedBlinkStick;

                if (PreviousDeviceSettings != null && PreviousDeviceSettings.Led != null)
                {
                    PreviousDeviceSettings.SendColor += BlinkStickSendColor;
                }

                UpdateUI();
            };
    
            UpdateUI();
        }

        void BlinkStickSendColor (object sender, SendColorEventArgs e)
        {
            Gtk.Application.Invoke(delegate {
                blinkstickemulatorwidget1.SetColor(e.Index, new Gdk.Color(e.R, e.G, e.B));
            });
        }
            
        public void RefreshDevices()
        {
            deviceComboboxWidget.LoadDevices(DataModel);
            UpdateUI();
        }

        private void UpdateUI()
        {
            buttonConfigure.Sensitive = deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null;
            buttonDelete.Sensitive = deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led == null;
            blinkstickinfowidget2.UpdateUI(deviceComboboxWidget.SelectedBlinkStick);

            blinkstickemulatorwidget1.EmulatedDevice = deviceComboboxWidget.SelectedBlinkStick == null ? 
                BlinkStickDeviceEnum.Unknown : deviceComboboxWidget.SelectedBlinkStick.BlinkStickDevice;
        }

        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            RefreshDevices();
        }

        protected void OnButtonDeleteClicked (object sender, EventArgs e)
        {
            DataModel.Devices.Remove(deviceComboboxWidget.SelectedBlinkStick);
            deviceComboboxWidget.LoadDevices(DataModel);
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

