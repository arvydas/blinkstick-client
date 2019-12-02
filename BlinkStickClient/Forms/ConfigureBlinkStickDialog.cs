using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Classes;
using BlinkStickDotNet;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    public partial class ConfigureBlinkStickDialog : Gtk.Dialog
    {
        public BlinkStickDeviceSettings DeviceSettings { get; set; }
        public ApplicationSettings ApplicationSettings;

        int mode = -1;

        public ConfigureBlinkStickDialog()
        {
            this.Build();
        }

        public void UpdateUI(Boolean initial = true)
        {
            if (DeviceSettings != null)
            {
                if (initial)
                {
                    mode = DeviceSettings.Led.GetMode();

                    entryName.Text = DeviceSettings.Led.InfoBlock1;
                    entryData.Text = DeviceSettings.Led.InfoBlock2;
                    hscaleLimitBrightness.Value = DeviceSettings.BrightnessLimit;

                    if (DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickFlex)
                    {
                        spinbuttonChannelR.Value = DeviceSettings.Led.GetLedCount();
                    }
                }

                switch (DeviceSettings.Led.BlinkStickDevice)
                {
                    case BlinkStickDeviceEnum.BlinkStick:
                        spinbuttonChannelR.Value = 1;
                        spinbuttonChannelG.Value = 0;
                        spinbuttonChannelB.Value = 0;

                        radiobuttonModeRGB.Sensitive = true;
                        radiobuttonModeRGB.Active = true;

                        radiobuttonModeInverse.Sensitive = false;
                        radiobuttonModeMultiLED.Sensitive = false;
                        radiobuttonModeMultiLEDMirror.Sensitive = false;
                        break;
                    case BlinkStickDeviceEnum.BlinkStickPro:
                        if (initial)
                        {
                            radiobuttonModeMultiLEDMirror.Sensitive = false;

                            if (mode == 2)
                            {
                                spinbuttonChannelR.Value = DeviceSettings.LedsR;
                                spinbuttonChannelG.Value = DeviceSettings.LedsG;
                                spinbuttonChannelB.Value = DeviceSettings.LedsB;
                            }
                            else
                            {
                                spinbuttonChannelR.Value = 1;
                                spinbuttonChannelG.Value = 1;
                                spinbuttonChannelB.Value = 1;
                            }

                            radiobuttonModeRGB.Active = mode == 0;
                            radiobuttonModeInverse.Active = mode == 1;
                            radiobuttonModeMultiLED.Active = mode == 2;
                        }
                        break;
                    case BlinkStickDeviceEnum.BlinkStickSquare:
                    case BlinkStickDeviceEnum.BlinkStickStrip:

                        spinbuttonChannelR.Value = 8;
                        spinbuttonChannelG.Value = 0;
                        spinbuttonChannelB.Value = 0;

                        radiobuttonModeRGB.Sensitive = false;
                        radiobuttonModeInverse.Sensitive = false;
                        radiobuttonModeMultiLED.Sensitive = true;
                        radiobuttonModeMultiLEDMirror.Sensitive = true;

                        if (initial)
                        {
                            radiobuttonModeMultiLED.Active = mode == 2;
                            radiobuttonModeMultiLEDMirror.Active = mode == 3;
                        }
                        break;
                    case BlinkStickDeviceEnum.BlinkStickNano:

                        spinbuttonChannelR.Value = 2;
                        spinbuttonChannelG.Value = 0;
                        spinbuttonChannelB.Value = 0;

                        radiobuttonModeRGB.Sensitive = false;
                        radiobuttonModeInverse.Sensitive = false;
                        radiobuttonModeMultiLED.Sensitive = true;
                        radiobuttonModeMultiLEDMirror.Sensitive = false;

                        radiobuttonModeMultiLED.Active = true;
                        break;
                    case BlinkStickDeviceEnum.BlinkStickFlex:

                        spinbuttonChannelG.Value = 0;
                        spinbuttonChannelB.Value = 0;

                        radiobuttonModeRGB.Sensitive = false;
                        radiobuttonModeInverse.Sensitive = false;
                        radiobuttonModeMultiLED.Sensitive = true;
                        radiobuttonModeMultiLEDMirror.Sensitive = false;

                        radiobuttonModeMultiLED.Active = true;
                        break;
                    default:
                        break;
                }

                spinbuttonChannelR.Sensitive = (DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickPro 
                    || DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickFlex) && radiobuttonModeMultiLED.Active;
                spinbuttonChannelG.Sensitive = DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickPro && radiobuttonModeMultiLED.Active;
                spinbuttonChannelB.Sensitive = DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickPro && radiobuttonModeMultiLED.Active;

                if (ApplicationSettings != null && !ApplicationSettings.AllowModeChange)
                {
                    radiobuttonModeRGB.Sensitive = false;
                    radiobuttonModeInverse.Sensitive = false;
                    radiobuttonModeMultiLED.Sensitive = false;
                    radiobuttonModeMultiLEDMirror.Sensitive = false;
                }
            }
        }

        protected void ModeRadioButtonToggled (object sender, EventArgs e)
        {
            UpdateUI(false);
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
            if (DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickFlex && (spinbuttonChannelR.Value < 1 || spinbuttonChannelR.Value > 32))
            {
                MessageBox.Show(this, "Please specify the number of LEDs in the range (1..32)", Gtk.MessageType.Error, Gtk.ButtonsType.Ok);
                return;
            }

            DeviceSettings.Led.InfoBlock1 = entryName.Text;
            DeviceSettings.Led.InfoBlock2 = entryData.Text;

            DeviceSettings.LedsR = (int)spinbuttonChannelR.Value;
            DeviceSettings.LedsG = (int)spinbuttonChannelG.Value;
            DeviceSettings.LedsB = (int)spinbuttonChannelB.Value;
            DeviceSettings.BrightnessLimit = (int)hscaleLimitBrightness.Value;

            //Set mode
            switch (DeviceSettings.Led.BlinkStickDevice)
            {
                case BlinkStickDeviceEnum.BlinkStick:
                    break;
                case BlinkStickDeviceEnum.BlinkStickPro:
                case BlinkStickDeviceEnum.BlinkStickSquare:
                case BlinkStickDeviceEnum.BlinkStickStrip:
                    if (radiobuttonModeRGB.Active && mode != 0)
                    {
                        DeviceSettings.Led.SetMode(0);
                    }
                    else if (radiobuttonModeInverse.Active && mode != 1)
                    {
                        DeviceSettings.Led.SetMode(1);
                    }
                    else if (radiobuttonModeMultiLED.Active && mode != 2)
                    {
                        DeviceSettings.Led.SetMode(2);
                    }
                    else if (radiobuttonModeMultiLEDMirror.Active && mode != 3)
                    {
                        DeviceSettings.Led.SetMode(3);
                    }

                    break;
                default:
                    break;
            }

            if (DeviceSettings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStickFlex)
            {
                DeviceSettings.Led.SetLedCount((byte)spinbuttonChannelR.Value);
            }

            this.Respond(Gtk.ResponseType.Ok);
        }
    }
}

