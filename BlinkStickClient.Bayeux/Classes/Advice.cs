#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStickClient.Bayeux library.
//
// BlinkStickClient.Bayeux library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStickClient.Bayeux library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStickClient.Bayeux library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;

namespace BlinkStickClient.Bayeux.Classes
{
	public class Advice
	{
		public String reconnect;
		public int interval;
		public int timeout;

		public Advice ()
		{
		}

		public override string ToString ()
		{
			return string.Format ("[Advice reconnect:{0} interval:{1} timeout:{2}]", 
			                      this.reconnect, this.interval, this.timeout);
		}
	}
}

