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
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Joshi.Utils.Imap;
using log4net;

namespace BlinkStickClient.Classes
{
	public class ImapEmailNotification : CustomNotification
	{
		public String Email;

		public String Password;

		public String ServerAddress;

		public ImapEmailNotification () : base()
		{
			log = LogManager.GetLogger("ImapEmail");
		}

		protected override void SetupDefaults ()
		{
			base.SetupDefaults ();

			Email = "";
			Password = "";
		}

		public override List<NotificationWidget> GetPages ()
		{
			List<NotificationWidget> pages = new List<NotificationWidget>();
			pages.Add(new ImapEmailSettingsWidget());
			return pages;
		}

		public override Boolean Check ()
		{
			Imap imap = new Imap();
			imap.Login(ServerAddress, Email, Password);

			imap.ExamineFolder("Inbox");

			ArrayList messages = new ArrayList();
			imap.SearchMessage(new string[] { "UNSEEN" }, true, messages);

			for (int i = messages.Count - 1; i >= 0; i--)
			{
				if ((string)messages[i] == "")
					messages.RemoveAt(i);
			}

			return messages.Count > 0;
		}

		public override CustomNotification Copy ()
		{
			ImapEmailNotification ev = new ImapEmailNotification();
			ev.CopyProperties(this);
			return ev;
		}

		public override void CopyProperties (CustomNotification source)
		{
			base.CopyProperties (source);

			if (source is ImapEmailNotification) {
				this.Email = (source as ImapEmailNotification).Email;
				this.Password = (source as ImapEmailNotification).Password;
				this.ServerAddress = (source as ImapEmailNotification).ServerAddress;
			}
		}

		public override string GetTypeName ()
		{
			return "IMAP eMail";
		}
	}
}

