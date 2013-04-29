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
using BlinkStick.Classes;

namespace BlinkStick.Classes
{
	public class NotificationWidget : Bin
	{
		public String Title {
			get {
				return GetTitle();
			}
		}

		public String LastError {
			get;
			set;
		}

		protected virtual String GetTitle ()
		{
			return "Noname";
		}

		public NotificationWidget ()
		{
		}

		public virtual void ObjectToControls (CustomNotification e)
		{
		}

		public virtual void ControlsToObject (CustomNotification e)
		{
		}

		public virtual Boolean Valid()
		{
			LastError = "";
			return true;
		}
	}
}

