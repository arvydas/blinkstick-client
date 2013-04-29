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

namespace BlinkStick.Utils
{
    public static class MessageBox
    {
        public static void Show(Window parent_window, string msg, MessageType messageType)
        {
        	MessageDialog md = Show(parent_window, msg, messageType, true); 
        	md.Destroy();
        }
        
        public static MessageDialog Show(Window parent_window, string msg, MessageType messageType, Boolean showModal)
        {
            MessageDialog md = new MessageDialog (parent_window, DialogFlags.Modal, messageType, ButtonsType.Ok, msg);
            if (showModal)
			{
	            md.Run ();
			}
			else
			{
	            md.Show ();
			}
			
			return md;
        }
        
	}
}

