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
using BlinkStick.Classes;
using System.Collections.Generic;
using Gtk;

namespace BlinkStick
{
	public partial class EditNotificationForm : Gtk.Dialog
	{
		public LedController Controller;
		public CustomNotification EditObject;
		public List<NotificationWidget> Pages;
		private List<DeviceEntity> DeviceEntities;
		private NotificationManager Manager;

		private String SelectedDeviceSerial {
			get {
				if (comboboxDevices.Active > -1) {
					return DeviceEntities [comboboxDevices.Active].Serial;
				} else {
					return "";
				}
			}
		}

		public EditNotificationForm ()
		{
			this.Build ();

			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
		}

		private void ObjectToControls ()
		{
			checkbuttonEnabled.Active = EditObject.Enabled;
			entryName.Text = EditObject.Name;
			comboboxBlinkSpeed.Active = (int)EditObject.BlinkSpeed;
			comboboxPriority.Active = (int)EditObject.Priority;
			colorbuttonColor.Color = new Gdk.Color (EditObject.Color.R, EditObject.Color.G, EditObject.Color.B);
			radiobuttonBlink.Active = EditObject.NotificationType == NotificationTypeEnum.Blink;
			radiobuttonPulse.Active = EditObject.NotificationType == NotificationTypeEnum.Pulse;
			radiobuttonMorph.Active = EditObject.NotificationType == NotificationTypeEnum.Morph;
			spinbuttonBlinkCount.Value = EditObject.BlinkCount;

			foreach (NotificationWidget page in Pages) {
				page.ObjectToControls (EditObject);
			}

			DeviceEntities = Manager.GetDeviceList();

			int index = -1;
			foreach (DeviceEntity entity in DeviceEntities) {
				index++;
				comboboxDevices.AppendText(entity.ToString());
				if (EditObject.Device == entity.Serial)
				{
					comboboxDevices.Active = index;
				}
			}
		}

		private void ControlsToObject()
		{
			ControlsToEvent(EditObject);
		}

		private void ControlsToEvent (CustomNotification e)
		{
			EditObject.Enabled = checkbuttonEnabled.Active;
			e.Name = entryName.Text;
			e.BlinkSpeed = (BlinkSpeedEnum)comboboxBlinkSpeed.Active;
			e.Priority = (NotificationPriorityEnum)comboboxPriority.Active;
			e.Color = RgbColor.FromGdkColor (colorbuttonColor.Color.Red, colorbuttonColor.Color.Green, colorbuttonColor.Color.Blue);

			if (radiobuttonBlink.Active) {
				e.NotificationType = NotificationTypeEnum.Blink;
			} else if (radiobuttonPulse.Active) {
				e.NotificationType = NotificationTypeEnum.Pulse;
			} else {
				e.NotificationType = NotificationTypeEnum.Morph;
			}

			e.BlinkCount = (Byte)spinbuttonBlinkCount.Value;

			foreach (NotificationWidget page in Pages) {
				page.ControlsToObject (e);
			}

			e.Device = SelectedDeviceSerial;

		}

		public Boolean ValidateForm ()
		{
			if (entryName.Text == "") {
				Utils.MessageBox.Show(this, "Name cannot be blank", MessageType.Error);
				return false;
			}

			foreach (NotificationWidget page in Pages) {
				if (!page.Valid())
				{
					Utils.MessageBox.Show(this, page.LastError, MessageType.Error);
					return false;
				}
			}

			return true;
		}

		private void SetupPages ()
		{
			Pages = EditObject.GetPages ();

			foreach (NotificationWidget page in Pages) {
				Label lbl = new Label();
				lbl.LabelProp = page.Title;
				notebook1.AppendPage(page, lbl);
			}

			notebook1.ShowAll();
		}

		public static Boolean ShowForm(CustomNotification tevent, NotificationManager manager)
		{
			Boolean result = false;
			EditNotificationForm form = new EditNotificationForm ();
			form.EditObject = tevent;
			form.Manager = manager;
			form.SetupPages();
			form.ObjectToControls();
			int response = form.Run ();
		
			while (true) {
				if ((ResponseType)response == ResponseType.Ok) {
					if (form.ValidateForm())
					{
						form.ControlsToObject();
						result = true;
						break;
					}
				}
				else if ((ResponseType)response == ResponseType.Cancel) {
					break;
				}
				else if ((ResponseType)response == ResponseType.Apply) {
					form.RunTest();
				}
				else
				{
					break;
				}

				response = form.Run ();
			}

			form.Destroy();

			return result;
		}

		public void RunTest ()
		{
			LedController controller = Manager.FindControllerBySerialNumber (SelectedDeviceSerial);
			if (controller != null) {
				CustomNotification tempEvent = new CustomNotification();
				ControlsToEvent(tempEvent);
				controller.ExecuteEvent(tempEvent);
			}
		}
	}
}

