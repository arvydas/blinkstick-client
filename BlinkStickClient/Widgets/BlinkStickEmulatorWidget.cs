using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Cairo;
using BlinkStickClient.Utils;
using BlinkStickDotNet;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class BlinkStickEmulatorWidget : Gtk.Bin
    {
        Gdk.Pixbuf display;

        /*
        private double Red = 0;
        private double Green = 0;
        private double Blue = 0;
        private double Alpha = 0;

        private Gdk.Color _LedColor;
        public Gdk.Color LedColor {
            get {
                return _LedColor;
            }
            set {
                if (!_LedColor.Equal(value))
                {
                    _LedColor = value;

                    HSLColor hsl = new HSLColor(RgbColor.FromGdkColor(value.Red, value.Green, value.Blue));

                    Alpha = hsl.Lightness;

                    if (hsl.Saturation == 0)
                    {
                        hsl.Lightness = 1;
                    }
                    else
                    {
                        if (Alpha > 0.5)
                        {
                            Alpha = 1;
                        }
                        else
                        {
                            Alpha = Alpha / 0.5;
                            hsl.Lightness = 0.5;
                        }
                    }

                    Red = hsl.Color.R / 255.0;
                    Green = hsl.Color.G / 255.0;
                    Blue = hsl.Color.B / 255.0;

                    QueueDraw();
                }
            }
        }
        */

        private BlinkStickDeviceEnum _EmulatedDevice;
        public BlinkStickDeviceEnum EmulatedDevice
        {
            get
            {
                return _EmulatedDevice;
            }
            set
            {
                if (_EmulatedDevice != value)
                {
                    _EmulatedDevice = value;
                    LoadDisplay();
                }
            }
        }

        private Dictionary<BlinkStickDeviceEnum, LedDefinition> LedDefinitions = 
            new Dictionary<BlinkStickDeviceEnum, LedDefinition>();

        private int LedSize = 50;

        private class LedDefinition
        {
            public List<PointD> Leds = new List<PointD>();

            public void AddLed(int x, int y)
            {
                Leds.Add(new PointD(x, y));
            }
        }

        private class LedColor
        {
            public double Red = 0;
            public double Green = 0;
            public double Blue = 0;
            public double Alpha = 0;
            public Gdk.Color Color = new Gdk.Color();
        }

        private LedDefinition CurrentDefinition
        {
            get
            {
                return LedDefinitions[EmulatedDevice];
            }
        }

        private List<LedColor> ColorBuffer = new List<LedColor>();

        double imageX = 0;
        double imageY = 0;
        double scale = 1;

        public int SelectedLed { get; private set; }
        private int FocussedLed = -1;

        public BlinkStickEmulatorWidget()
        {
            this.Build();

            SelectedLed = -1;

            //Build definitions
            LedDefinition blinkstickDefinition = new LedDefinition();
            blinkstickDefinition.AddLed(462, 68);
            LedDefinitions[BlinkStickDeviceEnum.BlinkStick] = blinkstickDefinition;

            LedDefinition blinkstickProDefinition = new LedDefinition();
            LedDefinitions[BlinkStickDeviceEnum.BlinkStickPro] = blinkstickProDefinition;

            LedDefinition blinkstickStripDefinition = new LedDefinition();
            blinkstickStripDefinition.AddLed(227, 45);
            blinkstickStripDefinition.AddLed(309, 45);
            blinkstickStripDefinition.AddLed(390, 45);
            blinkstickStripDefinition.AddLed(471, 45);
            blinkstickStripDefinition.AddLed(552, 45);
            blinkstickStripDefinition.AddLed(634, 45);
            blinkstickStripDefinition.AddLed(715, 45);
            blinkstickStripDefinition.AddLed(796, 45);
            LedDefinitions[BlinkStickDeviceEnum.BlinkStickStrip] = blinkstickStripDefinition;

            LedDefinition blinkstickSquareDefinition = new LedDefinition();
            blinkstickSquareDefinition.AddLed(218, 107);
            blinkstickSquareDefinition.AddLed(218, 176);
            blinkstickSquareDefinition.AddLed(163, 232);
            blinkstickSquareDefinition.AddLed(94, 232);
            blinkstickSquareDefinition.AddLed(37, 176);
            blinkstickSquareDefinition.AddLed(37, 107);
            blinkstickSquareDefinition.AddLed(94, 52);
            blinkstickSquareDefinition.AddLed(163, 52);

            LedDefinitions[BlinkStickDeviceEnum.BlinkStickSquare] = blinkstickSquareDefinition;

            LedDefinition blinkstickNanoDefinition = new LedDefinition();
            blinkstickNanoDefinition.AddLed(195, 52);
            blinkstickNanoDefinition.AddLed(195, 187);

            LedDefinitions[BlinkStickDeviceEnum.BlinkStickNano] = blinkstickNanoDefinition;

            LedDefinition blinkstickFlexDefinition = new LedDefinition();

            foreach (int i in new List<int>() { 70, 249, 429, 610 })
            {
                blinkstickFlexDefinition.AddLed(290, i);
                blinkstickFlexDefinition.AddLed(587, i);
                blinkstickFlexDefinition.AddLed(884, i);
                blinkstickFlexDefinition.AddLed(1182, i);
                blinkstickFlexDefinition.AddLed(1482, i);
                blinkstickFlexDefinition.AddLed(1779, i);
                blinkstickFlexDefinition.AddLed(2075, i);
                blinkstickFlexDefinition.AddLed(2372, i);
            }


            LedDefinitions[BlinkStickDeviceEnum.BlinkStickFlex] = blinkstickFlexDefinition;

            LedDefinitions[BlinkStickDeviceEnum.Unknown] = new LedDefinition();

            drawingareaMain.ExposeEvent += HandleExposeEvent;
            drawingareaMain.ButtonReleaseEvent += HandleButtonReleaseEvent;
            drawingareaMain.MotionNotifyEvent += HandleMotionNotifyEvent;

            LoadDisplay();
        }

        public void SetColor(Gdk.Color color)
        {
            DetermineDisplayColor(ColorBuffer[0], color);
            ColorBuffer[0].Color = color;
            drawingareaMain.QueueDraw();
        }

        public void SetColor(byte index, Gdk.Color color)
        {
            DetermineDisplayColor(ColorBuffer[index], color);
            ColorBuffer[index].Color = color;
            drawingareaMain.QueueDraw();
        }

        public Gdk.Color GetColor(int index = 0)
        {
            return ColorBuffer[index].Color;
        }

        private void DetermineDisplayColor(LedColor ledColor, Gdk.Color referenceColor)
        {
            HSLColor hsl = new HSLColor(referenceColor.ToRgbColor());

            ledColor.Alpha = hsl.Lightness;

            if (hsl.Saturation == 0)
            {
                hsl.Lightness = 1;
            }
            else
            {
                if (ledColor.Alpha > 0.5)
                {
                    ledColor.Alpha = 1;
                }
                else
                {
                    ledColor.Alpha = ledColor.Alpha * 2;
                    hsl.Lightness = 0.5;
                }
            }

            ledColor.Red = hsl.Color.R / 255.0;
            ledColor.Green = hsl.Color.G / 255.0;
            ledColor.Blue = hsl.Color.B / 255.0;
        }

        void HandleMotionNotifyEvent (object o, MotionNotifyEventArgs args)
        {
            int newLocation = FindLedAtLocation(args.Event.X, args.Event.Y);
            if (FocussedLed != newLocation)
            {
                FocussedLed = newLocation;
                drawingareaMain.QueueDraw();
            }
        }

        void HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
        {
            int newLocation = FindLedAtLocation(args.Event.X, args.Event.Y);
            if (SelectedLed != newLocation)
            {
                SelectedLed = newLocation;
                drawingareaMain.QueueDraw();
            }
        }

        private int FindLedAtLocation(double px, double py)
        {
            for (int i = 0; i < CurrentDefinition.Leds.Count; i++)
            {
                double x = imageX + CurrentDefinition.Leds[i].X / scale;
                double y = imageY + CurrentDefinition.Leds[i].Y / scale;
                double ledSize = LedSize / scale / 2;

                if (px >= x - ledSize && px <= x + ledSize &&
                    py >= y - ledSize && py <= y + ledSize)
                {
                    return i;
                }
            }

            return -1;
        }

        public void LoadDisplay()
        {
            ColorBuffer.Clear();

            int numberOfLeds = 64;

            switch (EmulatedDevice)
            {
                case BlinkStickDeviceEnum.BlinkStick:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick.png");
                    numberOfLeds = 1;
                    break;
                case BlinkStickDeviceEnum.BlinkStickPro:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick-pro.png");
                    numberOfLeds = 64;
                    break;
                case BlinkStickDeviceEnum.BlinkStickSquare:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick-square.png");
                    numberOfLeds = 8;
                    break;
                case BlinkStickDeviceEnum.BlinkStickStrip:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick-strip.png");
                    numberOfLeds = 8;
                    break;
                case BlinkStickDeviceEnum.BlinkStickNano:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick-nano.png");
                    numberOfLeds = 2;
                    break;
                case BlinkStickDeviceEnum.BlinkStickFlex:
                    display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.Resources.blinkstick-flex.png");
                    numberOfLeds = 32;
                    break;
                default:
                    display = null;
                    break;
            }

            while (ColorBuffer.Count < numberOfLeds)
            {
                ColorBuffer.Add(new LedColor());
            }

            drawingareaMain.QueueDraw();

            SelectedLed = -1;
        }

        void DrawLed(int index, Context cr)
        {
            PointD led = new PointD();
            led.X = imageX + CurrentDefinition.Leds[index].X / scale;
            led.Y = imageY + CurrentDefinition.Leds[index].Y / scale;
            double glowRadius = LedSize * 2 / scale;
            RadialGradient gradient = new RadialGradient(led.X, led.Y, 0, led.X, led.Y, glowRadius);
            gradient.AddColorStop(0, new Cairo.Color(ColorBuffer[index].Red, ColorBuffer[index].Green, ColorBuffer[index].Blue, ColorBuffer[index].Alpha));
            gradient.AddColorStop(0.15, new Cairo.Color(ColorBuffer[index].Red, ColorBuffer[index].Green, ColorBuffer[index].Blue, ColorBuffer[index].Alpha));
            gradient.AddColorStop(1, new Cairo.Color(ColorBuffer[index].Red, ColorBuffer[index].Green, ColorBuffer[index].Blue, 0));
            cr.Save();
            cr.Rectangle(led.X - glowRadius, led.Y - glowRadius, glowRadius * 2, glowRadius * 2);
            cr.Clip();
            cr.SetSource(gradient);
            cr.Mask(gradient);
            cr.Restore();
            gradient.Dispose();
        }

        void HandleExposeEvent (object o, Gtk.ExposeEventArgs args)
        {
            if (display == null)
                return;

            DrawingArea area = (DrawingArea) o;
            Cairo.Context cr =  Gdk.CairoHelper.Create(area.GdkWindow);

            int border = Math.Min((int)(Allocation.Width * 0.1), (int)(Allocation.Height * 0.1));

            scale = Math.Max (1.0 * display.Width / (Allocation.Width - border * 2), 1.0 * display.Height / (Allocation.Height - border * 2));

            Gdk.Pixbuf image = display.ScaleSimple ((int)(display.Width / scale), (int)(display.Height / scale), InterpType.Bilinear);

            imageX = (Allocation.Width - image.Width) / 2;
            imageY = (Allocation.Height - image.Height) / 2;

            Gdk.CairoHelper.SetSourcePixbuf(cr, image, imageX, imageY);
            cr.Paint();

            for (int i = 0; i < CurrentDefinition.Leds.Count; i++)
            {
                DrawLed(i, cr);
            }

            if (FocussedLed != -1)
            {
                cr.LineWidth = 5 / scale;
                cr.Save();
                cr.SetSourceRGB(0, 0, 1);
                cr.Arc(
                    imageX + CurrentDefinition.Leds[FocussedLed].X / scale, 
                    imageY + CurrentDefinition.Leds[FocussedLed].Y / scale, 
                    (LedSize + 5) / scale / 2, 
                    0, 
                    2 * Math.PI);
                cr.Stroke();
                cr.Restore();
            }

            if (SelectedLed != -1)
            {
                cr.LineWidth = 5 / scale;
                cr.Save();
                cr.SetSourceRGB(1, 0, 0);
                cr.Arc(
                    imageX + CurrentDefinition.Leds[SelectedLed].X / scale, 
                    imageY + CurrentDefinition.Leds[SelectedLed].Y / scale, 
                    LedSize / scale / 2, 
                    0, 
                    2 * Math.PI);
                cr.Stroke();
                cr.Restore();
            }

            image.Dispose();

            ((IDisposable) cr).Dispose();
        }
    }
}

