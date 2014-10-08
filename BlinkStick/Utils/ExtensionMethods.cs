using System;
using Gdk;
using BlinkStickDotNet;

namespace BlinkStickClient.Utils
{
    public static class GdkColorExcensions
    {
        public static double GetRed(this Color color) 
        {
            return (1.0 * color.Red / 0x10000);
        }

        public static double GetGreen(this Color color) 
        {
            return (1.0 * color.Green / 0x10000);
        }

        public static double GetBlue(this Color color) 
        {
            return (1.0 * color.Blue / 0x10000);
        }

        public static RgbColor ToRgbColor(this Color color)
        {
            return RgbColor.FromGdkColor(color.Red, color.Green, color.Blue);
        }

        public static void FromRgbColor(this Color color, RgbColor newColor)
        {
            color.Red = (ushort)(newColor.R * 0x100);
            color.Green = (ushort)(newColor.G * 0x100);
            color.Blue = (ushort)(newColor.B * 0x100);
        }
    }

}

