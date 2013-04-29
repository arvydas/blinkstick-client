#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.HID library.
//
// BlinkStick.HID library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.HID library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.HID library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using LibUsbDotNet;
//using LibUsbDotNet.Main;

namespace BlinkStick.Hid
{
	public static class BlinkstickDeviceFinder
	{
		public static Boolean IsUnix ()
		{
			int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128)) {
				return true;
            } else {
				return false;
            }
		}

		public static AbstractBlinkstickHid[] FindDevices ()
		{
			if (IsUnix ()) {
				return LinuxBlinkstickHid.AllDevices();
			} else {
				return WindowsBlinkstickHid.AllDevices();
			}
		}

		public static void FreeUsbResources ()
		{
			// Free usb resources
			if (IsUnix ()) {
				UsbDevice.Exit ();
			}
		}
	}
}

