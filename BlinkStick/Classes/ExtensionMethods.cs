using System;

namespace BlinkStickClient
{
    public static class ExtensionMethods
    {
        public static Gdk.Color ContrastColor(this Gdk.Color color)
        {
            byte d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.Red / 0x100 + 0.587 * color.Green / 0x100 + 0.114 * color.Blue / 0x100) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return new Gdk.Color(d, d, d);
        }
    }
}

