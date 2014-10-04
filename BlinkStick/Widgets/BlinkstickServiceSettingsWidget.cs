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
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BlinkstickServiceSettingsWidget : NotificationWidget
	{
		private static Gdk.Atom _atom = Gdk.Atom.Intern("CLIPBOARD", false);
    	private Gtk.Clipboard _clipBoard = Gtk.Clipboard.Get(_atom);

		public BlinkstickServiceSettingsWidget ()
		{
			this.Build ();
		}

		protected override string GetTitle ()
		{
			return "Blinkstick.com";
		}

		public override void ControlsToObject (CustomNotification e)
		{
			if (!(e is BlinkstickService)) {
				return;
			}
			BlinkstickService be = e as BlinkstickService;

			be.AccessCode = entryAccessCode.Text;
		}

		public override void ObjectToControls (CustomNotification e)
		{
			if (!(e is BlinkstickService)) {
				return;
			}
			BlinkstickService be = e as BlinkstickService;

			entryAccessCode.Text = be.AccessCode;
		}

		public override bool Valid ()
		{
			LastError = "";

			if (entryAccessCode.Text.Trim () == "") {
				LastError = "Access code cannot be blank";
				return false;
			}

			return true;
		}

		protected void OnButtonPasteClicked (object sender, EventArgs e)
		{
			entryAccessCode.Text = _clipBoard.WaitForText();
		}
	}
}

