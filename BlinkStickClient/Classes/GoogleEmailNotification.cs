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
using System.Net;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Xml;
using Gtk;
using System.Collections.Generic;
using log4net;
using System.Security.Cryptography.X509Certificates;

namespace BlinkStickClient.Classes
{
	public class GoogleEmailNotification : CustomNotification
	{
		public String Email;

		public String Password;

		public GoogleEmailNotification () : base()
		{
			log = LogManager.GetLogger("GoogleEmail");
		}

		protected override void SetupDefaults ()
		{
			base.SetupDefaults ();

			Email = "";
			Password = "";
		}

		public static bool Validator (object sender, X509Certificate certificate, X509Chain chain, 
                                      SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public override Boolean Check ()
		{
			try {
				ServicePointManager.ServerCertificateValidationCallback = Validator;
				WebRequest webGmailRequest = WebRequest.Create (@"https://mail.google.com/mail/feed/atom");
				webGmailRequest.PreAuthenticate = true;
		 
				NetworkCredential loginCredentials = new NetworkCredential (Email, Password);
				webGmailRequest.Credentials = loginCredentials;
		 
				WebResponse webGmailResponse = webGmailRequest.GetResponse ();
				Stream strmUnreadMailInfo = webGmailResponse.GetResponseStream ();
		 
				StringBuilder sbUnreadMailInfo = new StringBuilder ();
				byte[] buffer = new byte[8192];
				int byteCount = 0;
		 
				while ((byteCount = strmUnreadMailInfo.Read(buffer, 0, buffer.Length)) > 0)
					sbUnreadMailInfo.Append (System.Text.Encoding.ASCII.GetString (buffer, 0, byteCount));
		 
				XmlDocument UnreadMailXmlDoc = new XmlDocument ();
				UnreadMailXmlDoc.LoadXml (sbUnreadMailInfo.ToString ());
				XmlNodeList UnreadMailEntries = UnreadMailXmlDoc.GetElementsByTagName ("entry");

				return UnreadMailEntries.Count > 0;
			} catch (Exception e) {
				log.Error(e.Message);
				return false;
			}
		}

		public override List<NotificationWidget> GetPages ()
		{
			List<NotificationWidget> pages = new List<NotificationWidget>();
			pages.Add(new GoogleEmailSettingsWidget());
			return pages;
		}

		public override CustomNotification Copy ()
		{
			GoogleEmailNotification ev = new GoogleEmailNotification();
			ev.CopyProperties(this);
			return ev;
		}

		public override void CopyProperties (CustomNotification source)
		{
			base.CopyProperties (source);

			if (source is GoogleEmailNotification) {
				this.Email = (source as GoogleEmailNotification).Email;
				this.Password = (source as GoogleEmailNotification).Password;
			}
		}

		public override string GetTypeName ()
		{
			return "GMail";
		}
	}
}

