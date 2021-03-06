#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.Bayeux library.
//
// BlinkStick.Bayeux library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.Bayeux library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.Bayeux library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;

namespace BlinkStickClient.Bayeux.Classes
{
	/// <summary>
	/// This class (hopefully) covers all possible responses from the Bayeux server
	/// </summary>
	public class ServerResponse
	{
		public String clientId;
		public String channel;
		public Boolean successful;
		public Advice advice;
		public Dictionary<string, string> data;
		public String id;
		public String subscription;
		public String error;

		public ServerResponse ()
		{
		}
	}
}

