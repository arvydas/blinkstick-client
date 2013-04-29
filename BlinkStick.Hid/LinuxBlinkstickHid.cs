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
using LibUsbDotNet.Main;
using System.Runtime.InteropServices;
using MonoLibUsb;
using System.Collections.Generic;

namespace BlinkStick.Hid
{
	public class LinuxBlinkstickHid : AbstractBlinkstickHid
	{
		public UsbDevice device;

		public static UsbDeviceFinder deviceFinder = new UsbDeviceFinder(VendorId, ProductId);

		public LinuxBlinkstickHid ()
		{
		}

		protected override String GetSerial()
		{
			return device.Info.SerialString;
		}

		protected override String GetManufacturer()
		{
			return device.Info.ManufacturerString;
		}

		public override bool OpenDevice ()
		{
			// Find and open the usb device.
			if (device == null)
				device = UsbDevice.OpenUsbDevice (deviceFinder);

			// If the device is open and ready
			if (device == null)
				throw new Exception ("Device Not Found.");

			// If this is a "whole" usb device (libusb-win32, linux libusb)
			// it will have an IUsbDevice interface. If not (WinUSB) the 
			// variable will be null indicating this is an interface of a 
			// device.
			IUsbDevice wholeUsbDevice = device as IUsbDevice;
			if (!ReferenceEquals (wholeUsbDevice, null)) {
				// This is a "whole" USB device. Before it can be used, 
				// the desired configuration and interface must be selected.

				// Select config #1
				wholeUsbDevice.SetConfiguration (1);

				// Claim interface #0.
				wholeUsbDevice.ClaimInterface (0);
			} else {
				return false;
			}

			connectedToDriver = true;

			return true;
        }

		protected override void SetInfoBlock (byte id, byte[] data)
		{
			var deviceData = new byte[33];

			deviceData[0] = id;
			Array.Copy(data, 0, deviceData, 1, data.Length);


			IntPtr dat = Marshal.AllocHGlobal(33);
			Marshal.Copy(deviceData, 0, dat ,33);

			UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, id, 0, (byte)deviceData.Length);
			int transferred;

			device.ControlTransfer(ref packet, dat, deviceData.Length, out transferred);
		}

		public override Boolean GetInfoBlock (byte id, out byte[] data)
		{
			UsbSetupPacket packet = new UsbSetupPacket(0x80 | 0x20, (byte)1, id, 0, 33);

			byte[] deviceData = new byte[33];

			deviceData[0] = id;

			for (int i = 1; i< deviceData.Length; i++)
			{
				deviceData[i] = 0;
			}

			int transferred;

			data = new byte[32];

			if (device.ControlTransfer(ref packet, deviceData, deviceData.Length, out transferred))
			{
				Array.Copy(deviceData, 1, data, 0, data.Length);

				return true;
			}
			else
			{
				return false;
			}


		}

		/// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public override void CloseDevice()
        {
            if (device.IsOpen)
            {
                // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                // it exposes an IUsbDevice interface. If not (WinUSB) the 
                // 'wholeUsbDevice' variable will be null indicating this is 
                // an interface of a device; it does not require or support 
                // configuration and interface selection.
                IUsbDevice wholeUsbDevice = device as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // Release interface #0.
                    wholeUsbDevice.ReleaseInterface(0);
                }

                device.Close();
            }
            device = null;

			connectedToDriver = false;
        }

		public override Boolean GetLedColor (out byte r, out byte g, out byte b)
		{
			UsbSetupPacket packet = new UsbSetupPacket (0x80 | 0x20, (byte)1, (short)0x1, 0, 33);

			byte[] data = new byte[33];

			data [0] = 0x1;
			data [1] = 0x00;
			data [2] = 0x00;
			data [3] = 0x00;

			for (int i = 4; i< data.Length; i++) {
				data [i] = 0;
			}

			int transferred;

			if (device.ControlTransfer (ref packet, data, data.Length, out transferred)) {
				r = data [1];
				g = data [2];
				b = data [3];
				return true;
			} else {
				r = 0;
				g = 0;
				b = 0;
				return false;
			}
		}

		public override void SetLedColor (byte r, byte g, byte b)
		{
			var data = new byte[4];

			data[0] = 0x00;
			data[1] = r;
			data[2] = g;
			data[3] = b;

			IntPtr dat = Marshal.AllocHGlobal(4);
			Marshal.Copy(data,0,dat,4);

			UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, (short)0x01, 0, 0);
			int transferred;

			device.ControlTransfer(ref packet, dat, data.Length, out transferred);
		}

        public static LinuxBlinkstickHid[] AllDevices ()
		{
			List<LinuxBlinkstickHid> result = new List<LinuxBlinkstickHid>();

			foreach (UsbRegistry device in UsbDevice.AllDevices) {
				if (device.Vid == VendorId && device.Pid == ProductId)
				{
					LinuxBlinkstickHid blinkStick = new LinuxBlinkstickHid();
					blinkStick.device = device.Device;

					result.Add(blinkStick);
				}
			}

			return result.ToArray();
		}
	}
}

