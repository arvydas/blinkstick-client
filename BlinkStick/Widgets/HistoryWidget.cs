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
using System.ComponentModel;

namespace BlinkStick
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class HistoryWidget : Gtk.Bin
	{
		public NotificationManager Manager;

		public HistoryWidget ()
		{
			this.Build ();
		}

		public void Load ()
		{
			for (int i = Math.Max(0, Manager.History.Count - 10); i < Manager.History.Count - 1; i++) {
				Add (Manager.History[i]);
			}
		}

		public void Add(HistoryItem item)
		{
			HistoryItemWidget widget = new HistoryItemWidget();
			widget.Load (item);
			vboxHistoryItems.PackEnd (widget, true, true, 4);

			//vboxHistoryItems.ShowAll();
			widget.ShowAll();
		}
	}
}

