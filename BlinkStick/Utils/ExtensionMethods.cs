using System;
using Gdk;

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
    }

}

