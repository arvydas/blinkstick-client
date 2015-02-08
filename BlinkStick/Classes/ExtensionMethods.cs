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

        public static string TimeAgo(this DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("~{0} {1} ago", 
                    years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("~{0} {1} ago", 
                    months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return String.Format("~{0} {1} ago", 
                    span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("~{0} {1} ago", 
                    span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("~{0} {1} ago", 
                    span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return "<1 minute ago";
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }
    }
}

