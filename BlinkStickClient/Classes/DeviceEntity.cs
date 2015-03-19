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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlinkStickClient.Classes
{
	public class DeviceEntity
	{
		public String Serial;
		public String Name;
		
		[JsonConverter(typeof(StringEnumConverter))]
		public DeviceControlEnum Control {
			get;
			set;
		}

		[JsonIgnore]
		public Boolean Online;

		public DeviceEntity ()
		{
			this.Control = DeviceControlEnum.Normal;
		}

		public DeviceEntity (String serial, String name, Boolean online)
		{
			this.Serial = serial;
			this.Name = name;
			this.Online = online;
			this.Control = DeviceControlEnum.Normal;
		}

		public override string ToString ()
		{
			String result = "";

			if (this.Name == "" || this.Name == null) {
				result = this.Serial;
			} else {
				result = this.Name;
			}

			if (!this.Online) {
				result = result + " (unplugged)";
			}

			return result;
		}
	}
}

