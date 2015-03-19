#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Drawing;

namespace BlinkStickClient.Utils
{
	public class LinuxScreenCapture : AbstractScreenCapture
	{
		public LinuxScreenCapture ()
		{
		}

        // http://www.eggheadcafe.com/tutorials/aspnet/064b41e4-60bc-4d35-9136-368603bcc27a/7zip-lzma-inmemory-com.aspx

        protected static System.Drawing.Rectangle rectScreenBounds = GetScrBounds();
        //protected System.Drawing.Rectangle rectScreenBounds = System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
        protected static System.Drawing.Bitmap bmpScreenshot = new System.Drawing.Bitmap(1, 1);


        protected static System.Drawing.Rectangle GetXorgScreen()
        {
            int screen_width = 0;
            int screen_height = 0;

            IntPtr display = XorgAPI.XOpenDisplay(System.IntPtr.Zero);

            if (display == IntPtr.Zero)
            {
                Console.WriteLine("Error: Failed on XOpenDisplay.\n");
            }
            else
            {
                screen_width = XorgAPI.DisplayWidth(display, XorgAPI.XDefaultScreen(display));
                screen_height = XorgAPI.DisplayHeight(display, XorgAPI.XDefaultScreen(display));

                XorgAPI.XCloseDisplay(display);
                Console.WriteLine("Width: " + screen_width.ToString() + " Height: " + screen_height.ToString());
            } // End Else (display == IntPtr.Zero)

            return new System.Drawing.Rectangle(0, 0, screen_width, screen_height);
        } // End Function GetXorgScreen


        protected static System.Drawing.Rectangle GetScrBounds()
        {
            // Wouldn't be necessary if GetBounds on mono wasn't buggy.
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return GetXorgScreen();

            return System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
        } // End Function GetScrBounds


        // http://jalpesh.blogspot.com/2007/06/how-to-take-screenshot-in-c.html
        // Tools.Graphics.ScreenShot.GetScreenshot();
        public static System.Drawing.Bitmap GetScreenshot()
        {
            /*
            if (this.pictureBox1.Image != null)
                this.pictureBox1.Image.Dispose();
            */
            bmpScreenshot.Dispose();
            bmpScreenshot = new System.Drawing.Bitmap(rectScreenBounds.Width, rectScreenBounds.Height);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpScreenshot))
            {
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, rectScreenBounds.Size);
            } // End Using g

            return bmpScreenshot;
        } // End Function GetScreenshotImage

        // http://jalpesh.blogspot.com/2007/06/how-to-take-screenshot-in-c.html
        // Tools.Graphics.ScreenShot.SaveScreenshot(@"C:\Users\Stefan.Steiger.COR\Desktop\test.jpg");
        public static void SaveScreenshot(string strFileNameAndPath)
        {
            System.Drawing.Rectangle rectBounds = System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
            using (System.Drawing.Bitmap bmpScreenshotBitmap = new System.Drawing.Bitmap(rectBounds.Width, rectBounds.Height))
            {

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpScreenshotBitmap))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, rectBounds.Size);
                } // End Using g

                bmpScreenshotBitmap.Save(strFileNameAndPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            } // End Using

        } // End Sub SaveScreenshot


		public override bool ScreenCap (out byte r, out byte g, out byte b)
		{
			Bitmap bmp;
			Graphics gfx;
			
			try {
				// Make objects for storage/processing of images
				bmp = GetScreenshot();
				gfx = Graphics.FromImage(bmp);
				
				//gfx.CopyFromScreen(0, 0, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
			}
			catch {
				r = 0;
				g = 0;
				b = 0;
				return false;
			}

			// Analyze the screen capture:
			BitmapData srcData = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadOnly,
				PixelFormat.Format24bppRgb);
			
			int stride = srcData.Stride;
			
			IntPtr Scan0 = srcData.Scan0;
			
			long[] totals = new long[] { 0, 0, 0 };
			
			int width = bmp.Width;
			int height = bmp.Height;

			int step = 16;

			unsafe {
				byte* p = (byte*)(void*)Scan0;


				for (int y = 0; y < height; y += step) {
					for (int x = 0; x < width; x += step) {
						for (int color = 0; color < 3; color++) {
							int idx = (y * stride) + x * 3 + color;
							
							totals[color] += p[idx];
						}
					}
				}
			}
			
			byte avgR = (byte) (totals[2] / (width * height / step / step));
			byte avgG = (byte) (totals[1] / (width * height / step / step));
			byte avgB = (byte) (totals[0] / (width * height / step / step));
			
			r = avgR;
			b = avgB;
			g = avgG;
			
			bmp.UnlockBits(srcData);
			bmp.Dispose();
			gfx.Dispose();
			
			return true;
		}

    } // End  Class ScreenShot

}

