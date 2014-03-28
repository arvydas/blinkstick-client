using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Runtime.InteropServices;

namespace HidSharp
{
	public class LibusbHidStream : HidStream
	{
		private LibusbHidDevice _device;
		object _readSync = new object(), _writeSync = new object();
        byte[] _readBuffer, _writeBuffer;

		public LibusbHidStream ()
		{
		}

		internal void Init(string path, LibusbHidDevice device)
        {
			int handle;

			_device = device;
			DeviceInitAndOpen();
        }

		internal void DeviceInitAndOpen()
		{
			/*
			IUsbDevice wholeUsbDevice = _device.UsbDevice as IUsbDevice;

			if (!ReferenceEquals (wholeUsbDevice, null)) {
				// Select config #1
				wholeUsbDevice.SetConfiguration (1);
				// Claim interface #0.
				wholeUsbDevice.ClaimInterface (0);
			}
			*/
		}

		internal override void HandleFree()
		{
			/*
			NativeMethods.CloseHandle(ref _handle);
			NativeMethods.CloseHandle(ref _closeEventHandle);
			*/
		}

        public unsafe override void GetFeature(byte[] buffer, int offset, int count)
        {
            Throw.If.OutOfRange(buffer, offset, count);
			
			//HandleAcquireIfOpenOrFail();
			try
			{
				UsbSetupPacket packet = new UsbSetupPacket (0x80 | 0x20, buffer[offset], (short)0x1, 0, 33);

				int transferred;

				_device.UsbDevice.ControlTransfer (ref packet, buffer, count, out transferred);
			}
			finally
			{
				//HandleRelease();
			}
        }

        // Buffer needs to be big enough for the largest report, plus a byte
        // for the Report ID.
        public unsafe override int Read(byte[] buffer, int offset, int count)
        {
			throw new NotSupportedException(); // TODO
        }

        public unsafe override void SetFeature(byte[] buffer, int offset, int count)
        {
            Throw.If.OutOfRange(buffer, offset, count);

			var data = new byte[4];

			data[0] = 0x00;
			data[1] = 255;
			data[2] = 0;
			data[3] = 0;

			IntPtr dat = Marshal.AllocHGlobal(4);
			Marshal.Copy(data,0,dat,4);

			UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, (short)0x01, 0, (short)data.Length);
			int transferred;

			_device.UsbDevice.ControlTransfer(ref packet, dat, data.Length, out transferred);


			//HandleAcquireIfOpenOrFail();
			try
			{
				/*
				byte reportId = buffer[offset];

				buffer[offset] = 0;

				IntPtr dat = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, (int)0, dat, (int)count);

				UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, reportId, 0, (byte)buffer.Length);
				int transferred;

				_device.UsbDevice.ControlTransfer(ref packet, dat, buffer.Length, out transferred);
				*/
				/*
				byte reportId = buffer[offset];

				buffer[offset] = 0;

				IntPtr dat = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, dat, count);

				UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, reportId, 0, (byte)buffer.Length);
				//UsbSetupPacket packet = new UsbSetupPacket (0x20 | 0x09, reportId, (short)0x1, 0, 33);
				//UsbSetupPacket packet = new UsbSetupPacket(0x20, 0x09, reportId, (short)offset, (short)count);
				int transferred;

				_device.UsbDevice.ControlTransfer(ref packet, dat, buffer.Length, out transferred);
				*/
				if (transferred ==0) {};
			}
			finally
			{
				//HandleRelease();
			}
        }

        public unsafe override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(); // TODO
        }

        public override HidDevice Device
        {
            get { return _device; }
        }
	}
}

