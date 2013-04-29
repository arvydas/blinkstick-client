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
using System.Text.RegularExpressions;
using System.Text;

namespace BlinkStick.Hid
{
	public class AbstractBlinkstickHid
	{
		protected const int VendorId = 0x20A0;
        protected const int ProductId = 0x41E5;
        
		protected bool connectedToDriver = false;

		public Boolean Connected {
			get {
				return connectedToDriver;
			}
		}

		public String Serial {
			get {
				return GetSerial();
			}
		}

		public String ManufacturerName {
			get {
				return GetManufacturer();
			}
		}

        /// <summary>
		public String ProductName {
			get {
				return GetProductName();
			}
		}

		protected virtual String GetSerial()
		{
			return "Not Implemented";
		}

		protected virtual String GetManufacturer()
		{
			return "Not Implemented";
		}

		protected String GetProductName()
		{
			string data;
			GetInfoBlock(2, out data);
			
			return data;
		}

		private String _Name;
		public String Name {
			get {
				if (_Name == null) {
					GetInfoBlock (2, out _Name);
				}

				return _Name;
			}
			set {
				if (_Name != value)
				{
					_Name = value;
					SetInfoBlock(2, _Name);
				}
			}
		}

		private String _Data;
		public String Data {
			get {
				if (_Data == null) {
					GetInfoBlock (3, out _Data);
				}

				return _Data;
			}
			set {
				if (_Data != value)
				{
					_Data = value;
					SetInfoBlock(3, _Data);
				}
			}
		}

		public AbstractBlinkstickHid ()
		{

		}

		public virtual bool OpenDevice ()
		{
			throw new Exception("Not implemented");
        }

		protected virtual void SetInfoBlock (byte id, byte[] data)
		{
			throw new Exception("Not implemented");
		}

		public virtual Boolean GetInfoBlock (byte id, out byte[] data)
		{
			throw new Exception("Not implemented");
		}

		public void SetInfoBlock (byte id, string data)
		{
			SetInfoBlock(id, Encoding.ASCII.GetBytes(data));
		}

		public Boolean GetInfoBlock (byte id, out string data)
		{
			byte[] dataBytes;
			Boolean result = GetInfoBlock (id, out dataBytes);

			if (result) {
				for (int i = 0; i < dataBytes.Length; i++) {
					if (dataBytes [i] == 0) {
						Array.Resize (ref dataBytes, i);
						break;
					}
				}

				data = Encoding.ASCII.GetString (dataBytes);
			} else {
				data = "";
			}

			return result;
		}

		/// <summary>
        /// Set LED color
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
		public void SetLedColor(String color)
        {
            if (!IsValidColor(color))
                throw new Exception("Color value is invalid");

            SetLedColor(
                Convert.ToByte(color.Substring(1, 2), 16),
                Convert.ToByte(color.Substring(3, 2), 16),
                Convert.ToByte(color.Substring(5, 2), 16));
        }

		public static Boolean IsValidColor (String color)
		{
            return Regex.IsMatch(color, "^#[A-Fa-f0-9]{6}$");
		}

		public virtual void SetLedColor(byte r, byte g, byte b)
        {
			throw new Exception("Not implemented");
        }

		/// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public virtual void CloseDevice()
        {

        }

		public virtual Boolean GetLedColor (out byte r, out byte g, out byte b)
		{
			throw new Exception("Not implemented");
		}
	}
}

