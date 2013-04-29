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
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;
using BlinkStick.Bayeux;
using log4net;

namespace BlinkStick.Classes
{
	public class BlinkstickService : CustomNotification
	{
		#region Events
        // -------------- AccessCodeChanged ---------------
	    public class AccessCodeChangedEventArgs
	    {
	        public String OldValue;
	        public String NewValue;

	        public AccessCodeChangedEventArgs(String oldValue, String newValue)
	        {
				this.OldValue = oldValue;
				this.NewValue = newValue;
	        }
	    }

        public delegate void AccessCodeChangedEventHandler(object sender, AccessCodeChangedEventArgs e);

        public event AccessCodeChangedEventHandler AccessCodeChanged;

        protected void OnAccessCodeChanged(String oldValue, String newValue)
        {
            if (AccessCodeChanged != null)
            {
                AccessCodeChanged(this, new AccessCodeChangedEventArgs(oldValue, newValue));
            }
        }
		#endregion

		private string _AccessCode;
		public string AccessCode {
			get {
				return _AccessCode;
			}
			set {
				if (_AccessCode != value)
				{
					String oldValue = _AccessCode;
					_AccessCode = value;
					OnAccessCodeChanged(oldValue, value);
				}
			}
		}

		public BlinkstickService ()
		{
			log = LogManager.GetLogger("BlinkstickService");
		}

		protected override void SetupDefaults ()
		{
			base.SetupDefaults ();

			AccessCode = "";
		}

		public override bool Check ()
		{
			return false;
		}

		public override System.Collections.Generic.List<NotificationWidget> GetPages ()
		{
			List<NotificationWidget> pages = new List<NotificationWidget>();
			pages.Add(new BlinkstickServiceSettingsWidget());
			return pages;
		}

		public override CustomNotification Copy ()
		{
			BlinkstickService ev = new BlinkstickService();
			ev.CopyProperties(this);
			return ev;
		}

		public override void CopyProperties (CustomNotification source)
		{
			base.CopyProperties (source);

			if (source is BlinkstickService) {
				this.AccessCode = (source as BlinkstickService).AccessCode;
			}
		}

		public override string GetTypeName ()
		{
			return "BlinkStick.com";
		}

		public override bool GetSupportsPeriodicChecking ()
		{
			return false;
		}
	}
}

