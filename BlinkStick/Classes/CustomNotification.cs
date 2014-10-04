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
using System.Collections.Generic;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using log4net;
using BlinkStickDotNet;

namespace BlinkStickClient.Classes
{
	public class CustomNotification
	{
		protected ILog log;

		#region Events
		public event EventHandler ActiveStatusChanged;

		protected void OnActiveStatusChanged()
		{
			if (ActiveStatusChanged != null)
			{
				ActiveStatusChanged(this, new EventArgs());
			}
		}

		public event EventHandler EnabledStatusChanged;

		protected void OnEnabledStatusChanged()
		{
			if (EnabledStatusChanged != null)
			{
				EnabledStatusChanged(this, new EventArgs());
			}
		}
		#endregion

		private Boolean _Enabled;

		public Boolean Enabled {
			get {
				return _Enabled;
			}
			set {
				if (_Enabled != value)
				{
					_Enabled = value;
					OnEnabledStatusChanged();
				}
			}
		}

		public String Device {
			get;
			set;
		}

		public String Name {
			get;
			set;
		}

		public String Id {
			get;
			set;
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public BlinkSpeedEnum BlinkSpeed {
			get;
			set;
		}

		private Boolean _Active = false;

		[JsonIgnore]
		public Boolean Active {
			get {
				return _Active;
			}
			set {
				if (_Active != value)
				{
					_Active = value;
					OnActiveStatusChanged();
				}
			}
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public NotificationPriorityEnum Priority {
			get;
			set;
		}

		[JsonProperty("Color")]
		public String ColorString {
			get {
				return String.Format("#{0:X2}{1:X2}{2:X2}", this.Color.R, this.Color.G, this.Color.B);
			}
			set {
				this.Color = RgbColor.FromString(value);
			}
		}

		[JsonIgnore]
		public RgbColor Color {
			get;
			set;
		}

		public RgbColor VisibleColor {
			get 
			{
				return GetVisibleColor();
			}
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public NotificationTypeEnum NotificationType {
			get;
			set;
		}

		public Byte BlinkCount {
			get;
			set;
		}

		[JsonIgnore]
		public DateTime LastChecked = DateTime.Now.AddDays(-1);

		public CustomNotification ()
		{
			SetupDefaults();
		}

		public CustomNotification (String name) : base()
		{
			SetupDefaults();

			Name = name;
		}

		protected virtual void SetupDefaults ()
		{
			Device = "";
			Name = "";
			BlinkSpeed = BlinkSpeedEnum.Normal;
			Active = false;
			Id = "";
			Priority = NotificationPriorityEnum.Normal;
			Color = RgbColor.FromRgb(255, 255, 255);
			NotificationType = NotificationTypeEnum.Blink;
			BlinkCount = 1;
		}

		public override string ToString ()
		{
			return this.Name;
		}

		public virtual List<NotificationWidget> GetPages()
		{
			return new List<NotificationWidget>();
		}

		public virtual Boolean Check()
		{
			return false;
		}

		public virtual void CopyProperties (CustomNotification source)
		{
			this.Name = source.Name;
			this.Id = source.Id;
			this.BlinkSpeed = source.BlinkSpeed;
			this.Active = source.Active;
			this.Priority = source.Priority;
			this.Color = source.Color;
			this.NotificationType = source.NotificationType;
			this.BlinkCount = source.BlinkCount;
			this.Enabled = source.Enabled;
		}

		public virtual CustomNotification Copy()
		{
			CustomNotification ev = new CustomNotification();
			ev.CopyProperties(this);
			return ev;
		}

		public virtual String GetTypeName()
		{
			return "Custom";
		}

		public virtual Boolean GetSupportsPeriodicChecking()
		{
			return true;
		}

		public virtual void InitializeServices()
		{
		}

		public virtual void FinalizeServices()
		{
		}

		public virtual int GetCheckFrequency()
		{
			return 60;
		}

		public virtual RgbColor GetVisibleColor()
		{
			return Color;
		}
	}
}

