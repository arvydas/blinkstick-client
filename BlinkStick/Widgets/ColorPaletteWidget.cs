using System;
using System.Collections.Generic;
using Gtk;
using Cairo;
using Gdk;

namespace BlinkStickClient
{
    public class ColorPaletteWidget : DrawingArea
    {
        #region Events
        public event EventHandler<ColorClickedEventArgs> ColorClicked;

        protected void OnColorClicked(Gdk.Color color)
        {
            if (ColorClicked != null)
            {
                ColorClicked(this, new ColorClickedEventArgs(color));
            }
        }
        #endregion


        public String[] ColorList = new String[] {"black", "gray25", "gray50", "gray75", "white", "red", "green", "blue", "yellow", "orange", "brown", 
            "violet", "DarkViolet", "magenta", "SlateBlue1", "green1"};

        private List<Gdk.Color> CachedColors = new List<Gdk.Color>();

        public int SelectedIndex { get; private set; }

        public int TileSize { set; get; }
        public int TileSpacing { set; get; }

        public ColorPaletteWidget()
        {
            this.TileSize = 16;
            this.TileSpacing = 5;
            this.SelectedIndex = -1;

            AddEvents((int)(EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask));

            for (int i = 0; i < ColorList.Length; i++)
            {
                Gdk.Color c = new Gdk.Color();
                Gdk.Color.Parse(ColorList[i], ref c);

                CachedColors.Add(c);
            }
        }

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            Cairo.Context cr =  Gdk.CairoHelper.Create(this.GdkWindow);

            cr.LineWidth = 9;
            //cr.SetSourceRGB(0.7, 0.2, 0.0);
            int grow = 0;

            for (int i = 0; i < ColorList.Length; i++)
            {
                grow = 0;

                if (SelectedIndex == i)
                {
                    grow = 3;
                }

                DrawRoundedRectangle(cr, TileSpacing + (TileSize + TileSpacing) * i - grow,
                    TileSpacing - grow, TileSize + grow * 2, TileSize + grow * 2, 3);

                cr.SetSourceRGB(CachedColors[i].Red / (float)0x10000, CachedColors[i].Green / (float)0x10000, CachedColors[i].Blue / (float)0x10000);

                cr.FillPreserve ();

                /*
                c = c.ContrastColor();
                cr.SetSourceRGB(c.Red / (float)0x10000, c.Green / (float)0x10000, c.Blue / (float)0x10000);
                */
                cr.SetSourceRGB(0, 0, 0);

                cr.LineWidth = 1.2;
                cr.Stroke ();
            }

            PointD coords = drawButton(cr, TileSpacing + (TileSize + TileSpacing) * ColorList.Length, TileSpacing, "All Off", false);

            coords = drawButton(cr, coords.X, TileSpacing, "Custom", false);

            ((IDisposable) cr.GetTarget()).Dispose();                                      
            ((IDisposable) cr).Dispose();

            return true;
        }

        PointD drawButton(Cairo.Context cr, double x, double y, String text, Boolean selected)
        {
            cr.SetSourceRGB(0, 0, 0);
            cr.SelectFontFace("Georgia", FontSlant.Normal, FontWeight.Bold);
            cr.SetFontSize(11);
            TextExtents te = cr.TextExtents(text);

            int spacing = 5;
            int grow = selected ? 3 : 0;
            DrawRoundedRectangle(cr, 
                x - grow,
                y - grow, 
                te.Width + grow * 2 + spacing * 2, 
                TileSize + grow * 2, 
                3);
            cr.SetSourceRGB(1, 1, 1);
            cr.FillPreserve ();
            cr.SetSourceRGB(0, 0, 0);
            cr.LineWidth = 1.2;
            cr.Stroke ();

            cr.MoveTo(
                x + spacing  - te.XBearing,
                y - te.YBearing + (TileSize - te.Height) / 2);
            cr.ShowText(text);

            return new PointD(x + te.Width + spacing * 2 + TileSpacing, 0);
        }

        static void DrawRoundedRectangle (Cairo.Context gr, double x, double y, double width, double height, double radius)
        {
            gr.Save ();

            if ((radius > height / 2) || (radius > width / 2))
                radius = min (height / 2, width / 2);

            gr.MoveTo (x, y + radius);
            gr.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
            gr.LineTo (x + width - radius, y);
            gr.Arc (x + width - radius, y + radius, radius, -Math.PI / 2, 0);
            gr.LineTo (x + width, y + height - radius);
            gr.Arc (x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
            gr.LineTo (x + radius, y + height);
            gr.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
            gr.ClosePath ();
            gr.Restore ();
        }

        static double min (params double[] arr)
        {
            int minp = 0;
            for (int i = 1; i < arr.Length; i++)
                if (arr[i] < arr[minp])
                    minp = i;

            return arr[minp];
        }

        protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
        {
            if (SelectedIndex > -1)
            {
                OnColorClicked(CachedColors[SelectedIndex]);
            }

            return base.OnButtonReleaseEvent(evnt);
        }

        protected override bool OnMotionNotifyEvent(EventMotion evnt)
        {
            int index = (int)(evnt.X / (TileSize + TileSpacing));

            if (index < ColorList.Length && evnt.Y >= TileSpacing && evnt.Y <= TileSize + TileSpacing)
            {
                if (SelectedIndex != index)
                {
                    SelectedIndex = index;
                    QueueDraw();
                }
            }
            else if (SelectedIndex != -1)
            {
                SelectedIndex = -1;
                QueueDraw();
            }

            return base.OnMotionNotifyEvent(evnt);
        }
    }


    #region EventArgs subclasses
    public class ColorClickedEventArgs : EventArgs
    {
        public Gdk.Color Color;

        public ColorClickedEventArgs(Gdk.Color color)
        {
            this.Color = color;
        }
    }
    #endregion
}

