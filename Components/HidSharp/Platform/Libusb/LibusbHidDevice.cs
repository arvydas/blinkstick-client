using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Runtime.InteropServices;

namespace HidSharp
{
	public class LibusbHidDevice : HidDevice
	{
		private UsbRegistry deviceRegistry;
		private UsbDevice device;

		string _manufacturer;
        string _productName;
        string _serialNumber;
        byte[] _reportDescriptor;
        int _vid, _pid, _version;
        int _maxInput, _maxOutput, _maxFeature;
        bool _reportsUseID;
        string _path;


		public LibusbHidDevice (UsbRegistry key)
		{
			this.deviceRegistry = key;
		}

		public override HidStream Open()
        {
            var stream = new LibusbHidStream();
            try { stream.Init(_path, this); return stream; }
            catch { stream.Close(); throw; }
        }

        public override byte[] GetReportDescriptor()
        {
            return (byte[])_reportDescriptor.Clone();
        }

        internal unsafe bool GetInfo ()
		{
			device = deviceRegistry.Device;

			IUsbDevice wholeUsbDevice = device as IUsbDevice;

			if (!ReferenceEquals (wholeUsbDevice, null)) {
				// Select config #1
				wholeUsbDevice.SetConfiguration (1);
				// Claim interface #0.
				wholeUsbDevice.ClaimInterface (0);
			
			} else {
				return false;
			}

			var data = new byte[4];

			data[0] = 0x00;
			data[1] = 255;
			data[2] = 0;
			data[3] = 0;

			IntPtr dat = Marshal.AllocHGlobal(4);
			Marshal.Copy(data,0,dat,4);

			UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, (short)0x01, 0, 0);
			int transferred;

			device.ControlTransfer(ref packet, dat, data.Length, out transferred);

			if (device == null || !device.IsOpen)
				return false;

			if (device.Info == null)
				return false;

			_vid = device.Info.Descriptor.VendorID;
            _pid = device.Info.Descriptor.ProductID;
            _version = 0;
            _manufacturer = device.Info.ManufacturerString;
            _productName = device.Info.ProductString;
            _serialNumber = device.Info.SerialString;

            return true;
        }

        public override string DevicePath
        {
            get { return _path; }
        }

        public override int MaxInputReportLength
        {
            get { return _maxInput; }
        }

        public override int MaxOutputReportLength
        {
            get { return _maxOutput; }
        }

        public override int MaxFeatureReportLength
        {
            get { return _maxFeature; }
        }

        internal bool ReportsUseID
        {
            get { return _reportsUseID; }
        }

        public override string Manufacturer
        {
            get { return _manufacturer; }
        }

        public override int ProductID
        {
            get { return _pid; }
        }

        public override string ProductName
        {
            get { return _productName; }
        }

        public override int ProductVersion
        {
            get { return _version; }
        }

        public override string SerialNumber
        {
            get { return _serialNumber; }
        }

        public override int VendorID
        {
            get { return _vid; }
        }

		public UsbDevice UsbDevice {
			get {
				return this.device;
			}
		}
	}
}

