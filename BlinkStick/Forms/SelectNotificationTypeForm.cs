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
using Gtk;
using BlinkStickClient.Classes;

namespace BlinkStickClient
{
	public partial class SelectNotificationTypeForm : Gtk.Dialog
	{
		public SelectNotificationTypeForm ()
		{
			this.Build ();

			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
		}

		public static CustomNotification ShowForm()
		{
			CustomNotification result = null;
			SelectNotificationTypeForm form = new SelectNotificationTypeForm ();
			int response = form.Run ();
		
			if ((ResponseType)response == ResponseType.Ok) {
				if (form.radiobuttonCustom.Active)
				{
					result = new CustomNotification();
				}
				else if (form.radiobuttonGmail.Active)
				{
					result = new GoogleEmailNotification();
				}
				else if (form.radiobuttonImap.Active)
				{
					result = new ImapEmailNotification();
				}
				else if (form.radiobuttonBlinkstickService.Active)
				{
					result = new BlinkstickService();
				}
				else if (form.radiobuttonCpuUsage.Active)
				{
					result = new CpuUsageNotification();
				}
				else if (form.radiobuttonAmbilight.Active)
				{
					result = new AmbiLightNotification();
				}
			}

			form.Destroy();

			return result;
		}
	}
}

