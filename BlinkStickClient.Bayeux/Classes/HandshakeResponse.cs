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
using System.Collections.Generic;

namespace BlinkStickClient.Bayeux.Classes
{
	public class HandshakeResponse
	{
		public String channel;
		public Boolean successful;
		public String Version;
		public IList<string> supportedConnectionTypes;
		public String clientId;
		public Advice advice;

		public HandshakeResponse ()
		{
		}
	}
}

