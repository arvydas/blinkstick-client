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
using BlinkStickClient.Classes;
using Gtk;
using BlinkStickDotNet;

namespace BlinkStickClient
{
	public partial class BlinkStickTestForm : Gtk.Dialog
	{
		private LedController SelectedController;
		public NotificationManager Manager;
		private List<DeviceEntity> DeviceEntities;
        private CustomNotification TestNotification;

		public BlinkStickTestForm ()
		{
			this.Build ();

			colorSelection.HasOpacityControl = false;
			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
		}

		public static void ShowForm(NotificationManager manager)
		{
			BlinkStickTestForm form = new BlinkStickTestForm ();
			form.Manager = manager;
			form.PopulateForm();
			form.Run ();
			form.Destroy();
		}

		public void PopulateForm ()
		{
			//for (int i = 0; i < comboboxDevices.Model.IterNChildren(); i++)
			//	comboboxDevices.RemoveText(i);
			/*
			List<String> deviceNames = new List<string> ();
			foreach (LedController controller in Manager.Controllers) {
				deviceNames.Add (controller.DeviceVisibleName);
			}
			deviceNames.Sort ();
			*/
			string previousDeviceSerial = "";
			if (comboboxDevices.Active >= 0)
				previousDeviceSerial = DeviceEntities[comboboxDevices.Active].Serial;

			ListStore store = new ListStore(typeof (string));
			comboboxDevices.Model = store;

			DeviceEntities = Manager.GetOnlineDeviceList();

			int previousDeviceIndex = -1;
			int index = -1;
			foreach (DeviceEntity entity in DeviceEntities) {
				store.AppendValues(entity.ToString());

				index++;

				if (entity.ToString() == previousDeviceSerial)
				{
					previousDeviceIndex = index;
				}
			}

			SelectedController = null;

			if (store.IterNChildren() >= 1 && previousDeviceIndex == -1)
			{
				comboboxDevices.Active = 0;
			}
			else if (previousDeviceIndex >= 0)
			{
				comboboxDevices.Active = previousDeviceIndex;
			}
			else
			{
				UpdateFormComponents();
			}
		}

		public void UpdateFormComponents()
		{
			cbAutoSetColor.Sensitive = SelectedController != null;
			colorSelection.Sensitive = SelectedController != null;
			buttonSetColor.Sensitive = SelectedController != null && !cbAutoSetColor.Active;
			buttonSwitchToColor.Sensitive = SelectedController != null && !cbAutoSetColor.Active;
			buttonPulseColor.Sensitive = SelectedController != null;
		}

		protected void OnComboboxDevicesChanged (object sender, EventArgs e)
		{
			SelectedController = Manager.FindControllerBySerialNumber(DeviceEntities[comboboxDevices.Active].Serial);
			UpdateFormComponents();
		}

		protected void OnButtonAllOffClicked (object sender, EventArgs e)
		{
			foreach (LedController controller in Manager.Controllers) {
				controller.SendColor(0,0,0);
			}
		}


		protected void OnColorSelectionColorChanged (object sender, System.EventArgs e)
		{
			if (this.cbAutoSetColor.Active) {
				SelectedController.SendColor(colorSelection.CurrentColor);
			}
		}

		protected void OnButtonSetColorClicked (object sender, System.EventArgs e)
		{
			SelectedController.SendColor(colorSelection.CurrentColor);
		}

		protected void OnButtonPulseColorClicked (object sender, System.EventArgs e)
		{
            if (TestNotification == null)
            {
                TestNotification = new CustomNotification();
                TestNotification.NotificationType = NotificationTypeEnum.Pulse;
                TestNotification.BlinkCount = 1;
                TestNotification.BlinkSpeed = BlinkSpeedEnum.Normal;
            }

            TestNotification.Color = RgbColor.FromGdkColor(
                colorSelection.CurrentColor.Red, 
                colorSelection.CurrentColor.Green, 
                colorSelection.CurrentColor.Blue);

            SelectedController.ExecuteEvent(TestNotification);
		}

		protected void OnButtonSwitchToColorClicked (object sender, System.EventArgs e)
		{
			SelectedController.MorphToColor(
				RgbColor.FromGdkColor(
					colorSelection.CurrentColor.Red, 
					colorSelection.CurrentColor.Green, 
					colorSelection.CurrentColor.Blue));
		}

		protected void OnCbAutoSetColorToggled (object sender, EventArgs e)
		{
			UpdateFormComponents();
		}
	}
}

