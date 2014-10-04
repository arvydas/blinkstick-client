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
using BlinkStickClient.Classes;

namespace BlinkStickClient
{
	public partial class ImapEmailSettingsWidget : NotificationWidget
	{
		public ImapEmailSettingsWidget ()
		{
			this.Build ();
		}

		protected override string GetTitle ()
		{
			return "Imap";
		}

		public override void ControlsToObject (CustomNotification e)
		{
			if (!(e is ImapEmailNotification)) {
				return;
			}
			ImapEmailNotification ge = e as ImapEmailNotification;

			ge.Email = entryUsername.Text;
			ge.Password = entryPassword.Text;
			ge.ServerAddress = entryServerAddress.Text;
		}

		public override void ObjectToControls (CustomNotification e)
		{
			if (!(e is ImapEmailNotification)) {
				return;
			}
			ImapEmailNotification ge = e as ImapEmailNotification;

			entryUsername.Text = ge.Email;
			entryPassword.Text = ge.Password;
			entryServerAddress.Text = ge.ServerAddress;
		}

		public override bool Valid ()
		{
			LastError = "";

			if (entryServerAddress.Text.Trim () == "") {
				LastError = "Server address cannot be blank";
				return false;
			}

			if (entryUsername.Text.Trim () == "") {
				LastError = "User name cannot be blank";
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

