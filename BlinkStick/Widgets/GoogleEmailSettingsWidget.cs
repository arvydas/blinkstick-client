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

namespace BlinkStick
{
	public partial class GoogleEmailSettingsWidget : NotificationWidget
	{
		protected override string GetTitle ()
		{
			return "GMail";
		}

		public GoogleEmailSettingsWidget ()
		{
			this.Build ();
		}

		public override void ControlsToObject (CustomNotification e)
		{
			if (!(e is GoogleEmailNotification)) {
				return;
			}
			GoogleEmailNotification ge = e as GoogleEmailNotification;

			ge.Email = entryUsername.Text;
			ge.Password = entryPassword.Text;
		}

		public override void ObjectToControls (CustomNotification e)
		{
			if (!(e is GoogleEmailNotification)) {
				return;
			}
			GoogleEmailNotification ge = e as GoogleEmailNotification;

			entryUsername.Text = ge.Email;
			entryPassword.Text = ge.Password;
		}

		public override bool Valid ()
		{
			LastError = "";

			if (entryUsername.Text.Trim () == "") {
				LastError = "Email cannot be blank";
				return false;
			}

			if (entryPassword.Text.Trim () == "") {
				LastError = "Password cannot be blank";
				return false;
			}

			return true;
		}
	}
}

