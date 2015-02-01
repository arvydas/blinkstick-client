using System;
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

        public BlinkStickEmulatorWidget()
        {
            this.Build();

            LedColor = new Gdk.Color(0, 0, 0);

            drawingareaMain.ExposeEvent += HandleExposeEvent;
            //display = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "BlinkStick.png"));
            display = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.blinkstick.png");
        }

        void HandleExposeEvent (object o, Gtk.ExposeEventArgs args)
        {
            DrawingArea area = (DrawingArea) o;
            Cairo.Context cr =  Gdk.CairoHelper.Create(area.GdkWindow);

            int border = Math.Min((int)(Allocation.Width * 0.2), (int)(Allocation.Height * 0.2));

            double scale = Math.Max (1.0 * display.Width / (Allocation.Width - border * 2), 1.0 * display.Height / (Allocation.Height - border * 2));

            Gdk.Pixbuf image = display.ScaleSimple ((int)(display.Width / scale), (int)(display.Height / scale), InterpType.Bilinear);

            double imageX = (Allocation.Width - image.Width) / 2;
            double imageY = (Allocation.Height - image.Height) / 2;

            Gdk.CairoHelper.SetSourcePixbuf(cr, image, imageX, imageY);
            cr.Paint();

            PointD led = new PointD();
            led.X = imageX + 466 / scale;
            led.Y = imageY + 69 / scale;

            double glowRadius = image.Height * 0.75;

            RadialGradient gradient = new RadialGradient(led.X, led.Y, 0, led.X, led.Y, glowRadius);
            gradient.AddColorStop(0, new Cairo.Color(Red, Green, Blue, Alpha));
            gradient.AddColorStop(0.15, new Cairo.Color(Red, Green, Blue, Alpha));
            gradient.AddColorStop(1, new Cairo.Color(Red, Green, Blue, 0));

            cr.Save();
            cr.Rectangle(led.X - glowRadius, led.Y - glowRadius, glowRadius * 2, glowRadius * 2);
            cr.Clip();
            cr.SetSource(gradient);
            cr.Mask(gradient);
            cr.Restore();

            image.Dispose();
            gradient.Dispose();
            ((IDisposable) cr).Dispose();

        }
    }
}

