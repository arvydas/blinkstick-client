using System;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class BlinkStickInfoWidget : Gtk.Bin
    {
        public BlinkStickInfoWidget()
        {
            this.Build();

            UpdateUI(null);
        }

        public void UpdateUI(BlinkStickDeviceSettings settings)
        {
            if (settings == null)
            {
                labelConnectedInfo.Text = "";
                labelSerialNumberInfo.Text = "";
                labelManufacturerInfo.Text = "";
                labelProductInfo.Text = "";
                labelModeInfo.Text = "";
            }
            else
            {
                labelConnectedInfo.Text = settings.Led != null ? "Yes" : "No";
                labelSerialNumberInfo.Text = settings.Serial;

                switch (settings.BlinkStickDevice)
                {
                    case BlinkStickDeviceEnum.Unknown:
                        labelProductInfo.Text = "Unknown";
                        break;
                    case BlinkStickDeviceEnum.BlinkStick:
                        labelProductInfo.Text = "BlinkStick";
                        break;
                    case BlinkStickDeviceEnum.BlinkStickPro:
                        labelProductInfo.Text = "BlinkStick Pro";
                        break;
                    case BlinkStickDeviceEnum.BlinkStickStrip:
                        labelProductInfo.Text = "BlinkStick Strip";
                        break;
                    case BlinkStickDeviceEnum.BlinkStickSquare:
                        labelProductInfo.Text = "BlinkStick Square";
                        break;
                    case BlinkStickDeviceEnum.BlinkStickNano:
                        labelProductInfo.Text = "BlinkStick Nano";
                        break;
                    case BlinkStickDeviceEnum.BlinkStickFlex:
                        labelProductInfo.Text = "BlinkStick Flex";
                        break;
                    default:
                        break;
                }


                if (settings.Led != null)
                {
                    labelManufacturerInfo.Text = settings.Led.ManufacturerName;

                    if (settings.Led.BlinkStickDevice == BlinkStickDeviceEnum.BlinkStick)
                    {
                        labelModeInfo.Text = "RGB";
                    }
                    else
                    {
                        try
                        {
                            switch (settings.Led.GetMode())
                            {
                                case 0:
                                    labelModeInfo.Text = "RGB";
                                    break;
                                case 1:
                                    labelModeInfo.Text = "Inverse";
                                    break;
                                case 2:
                                    labelModeInfo.Text = "Multi-LED";
                                    break;
                                case 3:
                                    labelModeInfo.Text = "Multi-LED Mirror";
                                    break;
                                default:
                                    labelModeInfo.Text = "Unknown";
                                    break;
                            }
                        }
                        catch
                        {
                            labelModeInfo.Text = "Error";
                        }
                    }
                }
                else
                {
                    labelManufacturerInfo.Text = "";
                    labelProductInfo.Text = "";
                    labelModeInfo.Text = "";
                }
            }
        }
    }
}

