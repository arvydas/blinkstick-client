using System;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class OverviewWidget : Gtk.Bin
    {
        public OverviewWidget()
        {
            this.Build();
            //Gdk.Pixbuf image = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "BlinkStick.png"));
            Gdk.Pixbuf image = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.blinkstick.png");
            image = image.ScaleSimple(image.Width / 2, image.Height / 2, Gdk.InterpType.Nearest);
            imageBlinkStickPreview.Pixbuf = image;

            /*
            HollyLibrary.HColorPickerWidget picker = new HollyLibrary.HColorPickerWidget();
            vbox4.PackEnd(picker);
            */

            hbox1.PackStart(new ColorPaletteWidget());
        }
    }
}

