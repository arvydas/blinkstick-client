using System;
using Gtk;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
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

        private global::BlinkStickClient.DeviceComboboxWidget deviceComboboxWidget;
        private global::BlinkStickClient.BlinkStickEmulatorWidget blinkstickemulatorwidget1;
        private global::BlinkStickClient.BlinkStickInfoWidget blinkstickinfowidget2;


        public OverviewWidget()
        {
            this.Build();

            this.deviceComboboxWidget = new global::BlinkStickClient.DeviceComboboxWidget ();
            this.deviceComboboxWidget.Events = ((global::Gdk.EventMask)(256));
            this.deviceComboboxWidget.Name = "deviceComboboxWidget";
            this.deviceComboboxWidget.AutoSelectDevice = true;
            this.hboxMiniMenu.Add (this.deviceComboboxWidget);
            global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hboxMiniMenu [this.deviceComboboxWidget]));
            w2.Position = 1;
            w2.Expand = false;
            w2.Fill = false;

            this.blinkstickemulatorwidget1 = new global::BlinkStickClient.BlinkStickEmulatorWidget ();
            this.blinkstickemulatorwidget1.HeightRequest = 200;
            this.blinkstickemulatorwidget1.Events = ((global::Gdk.EventMask)(256));
            this.blinkstickemulatorwidget1.Name = "blinkstickemulatorwidget1";

            this.blinkstickemulatorwidget1 = new global::BlinkStickClient.BlinkStickEmulatorWidget ();
            this.blinkstickemulatorwidget1.HeightRequest = 200;
            this.blinkstickemulatorwidget1.Events = ((global::Gdk.EventMask)(256));
            this.blinkstickemulatorwidget1.Name = "blinkstickemulatorwidget1";
            this.vbox4.Add (this.blinkstickemulatorwidget1);
            global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.blinkstickemulatorwidget1]));
            w10.Position = 1;

            this.blinkstickinfowidget2 = new global::BlinkStickClient.BlinkStickInfoWidget ();
            this.blinkstickinfowidget2.Events = ((global::Gdk.EventMask)(256));
            this.blinkstickinfowidget2.Name = "blinkstickinfowidget2";

            this.vbox4.Add (this.blinkstickinfowidget2);
            global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.blinkstickinfowidget2]));
            w15.Position = 4;
            w15.Expand = false;
            w15.Fill = false;

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

            colorPaletteWidget.AllOffClicked += (object sender, EventArgs e) => {
                if (deviceComboboxWidget.SelectedBlinkStick != null && deviceComboboxWidget.SelectedBlinkStick.Led != null)
                {
                    deviceComboboxWidget.SelectedBlinkStick.TurnOff();
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
            if (MainWindow.ConfirmDelete())
            {
                DataModel.Devices.Remove(deviceComboboxWidget.SelectedBlinkStick);
                deviceComboboxWidget.LoadDevices(DataModel);
            }
        }

        protected void OnButtonConfigureClicked (object sender, EventArgs e)
        {
            ConfigureBlinkStickDialog dialog = new ConfigureBlinkStickDialog();
            dialog.DeviceSettings = deviceComboboxWidget.SelectedBlinkStick;
            dialog.UpdateUI();
            dialog.Run();
            dialog.Destroy();

            UpdateUI();
        }
    }
}

