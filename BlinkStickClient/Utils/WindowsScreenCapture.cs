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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BlinkStickClient.Utils
{
	public class WindowsScreenCapture : AbstractScreenCapture
	{
		public WindowsScreenCapture ()
		{
		}

		public override bool ScreenCap (out byte r, out byte g, out byte b)
		{
			Bitmap bmp;
			Graphics gfx;
			
			try {
				// Make objects for storage/processing of images
				bmp = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height, PixelFormat.Format24bppRgb);
				gfx = Graphics.FromImage(bmp);
				
				gfx.CopyFromScreen(0, 0, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
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

			int step = 8;

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
 	}
}

