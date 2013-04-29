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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using BlinkStick.Hid;
using BlinkStick.Bayeux;
using BlinkStick.Classes;
using BlinkStick.Utils;
using HidLibrary;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using log4net;

namespace BlinkStick.Classes
{
	public class NotificationManager
	{
		protected static readonly ILog log = LogManager.GetLogger("NotificationManager");	

		#region Events
        // -------------- NotificationUpdated ---------------
	    public class NotificationUpdatedEventArgs
	    {
	        public CustomNotification Notification;

	        public NotificationUpdatedEventArgs(CustomNotification notification)
	        {
	            this.Notification = notification;
	        }
	    }

        public delegate void NotificationUpdatedEventHandler(object sender, NotificationUpdatedEventArgs e);

        public event NotificationUpdatedEventHandler NotificationUpdated;

        protected void OnNotificationUpdated(CustomNotification notification)
        {
            if (NotificationUpdated != null)
            {
                NotificationUpdated(this, new NotificationUpdatedEventArgs(notification));
            }
        }
		#endregion

		public List<CustomNotification> Notifications = new List<CustomNotification>();

		[JsonIgnore]
		public List<HistoryItem> History = new List<HistoryItem>();

		public List<DeviceEntity> KnownDevices = new List<DeviceEntity>();

        private String FileName;
        private String BackupFileName;

        BackgroundWorker monitorWorker;
        BackgroundWorker animationWorker;
		BackgroundWorker ambilightWorker;

		BayeuxClient client;

		const int DelayBetweenEvents = 1000;

		[JsonIgnore]
		public List<LedController> Controllers = new List<LedController>();

		public LedController FindControllerBySerialNumber (String serialNumber)
		{
			foreach (LedController controller in Controllers) {
				if (controller.Device.Serial == serialNumber)
				{
					return controller;
				}
			}

			return null;
		}

		public DeviceEntity FindKnownDeviceBySerialNumber (String serialNumber)
		{
			foreach (DeviceEntity device in KnownDevices) {
				if (device.Serial == serialNumber)
				{
					return device;
				}
			}

			return null;
		}

		public BlinkstickService FindBlinkstickNotificaitonByAccessCode (string accessCode)
		{
			foreach (CustomNotification notification in Notifications) {
				if (notification is BlinkstickService && ((BlinkstickService)notification).AccessCode == accessCode)
				{
					return notification as BlinkstickService;
				}
			}

			return null;
		}

		[JsonIgnore]
		public Boolean Running 
		{
			get;
			private set;
		}

		public String ApiAccessAddress {
			get;
			set;
		}

		private const String DefaultApiAccessAddress = "http://live.blinkstick.com:9292/faye";

		private String CurrentApiAccessAddress {
			get {
				if (ApiAccessAddress == "" || ApiAccessAddress == null)
				{
					return DefaultApiAccessAddress;
				}
				else
				{
					return ApiAccessAddress;
				}
			}
		}

		public Boolean ShouldSerialzeApiAccessAddress()
		{
			return ApiAccessAddress != null && ApiAccessAddress != "";
		}

		[JsonIgnore]
		public String DefaultSettingsFolder
        {
            get 
			{ 
				return System.IO.Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
				    "Agile Innovative", 
					"BlinkStick"); 
			}
        } 

		public NotificationManager ()
		{
			if (!Directory.Exists (DefaultSettingsFolder))
				Directory.CreateDirectory (DefaultSettingsFolder);

			FileName = Path.Combine (DefaultSettingsFolder, "settings.json");
			BackupFileName = Path.Combine (DefaultSettingsFolder, "settings.~json");

			ApiAccessAddress = null;
		}

		public void UpdateControllers ()
		{
			foreach (LedController controller in Controllers) {
				controller.Checked = false;
			}

			foreach (LedController controller in LedController.AllControllers()) {
				if (controller.Device.Serial == "")
					continue;

				LedController knownController = FindControllerBySerialNumber(controller.Device.Serial);
				if (knownController == null)
				{
					Controllers.Add (controller);
					log.InfoFormat("Added device {0}", controller.Device.Serial);
					controller.Checked = true;
				}
				else
				{
					knownController.Checked = true;
				}

				DeviceEntity knowDeviceEntity = FindKnownDeviceBySerialNumber(controller.Device.Serial);
				if (knowDeviceEntity == null)
				{
					knowDeviceEntity = new DeviceEntity(controller.Device.Serial, controller.Device.Name, true);
					KnownDevices.Add(knowDeviceEntity);
				}
				else
				{
					knowDeviceEntity.Name = controller.Device.Name;
				}

				controller.DataEntity = knowDeviceEntity;
			}
			List<LedController> copy = new List<LedController>();
			copy.AddRange(Controllers);

			foreach (LedController controller in copy) {
				if (!controller.Checked)
				{
					Controllers.Remove(controller);
					log.InfoFormat("Removed device {0}", controller.Device.Serial);
				}
			}
		}

        public void Load()
        {
            if (!Load(FileName))
            {
                Load(BackupFileName);
            }

            if (!File.Exists(FileName))
                Save();
        }

        private Boolean Load(String SettingsFileName)
        {
            if (File.Exists(SettingsFileName))
            {
                try
                {
					NotificationManager em;

					JsonSerializer serializer = new JsonSerializer ();
					serializer.TypeNameHandling = TypeNameHandling.Auto;
					using (TextReader tr = new StreamReader(SettingsFileName)) {
						em = (NotificationManager)serializer.Deserialize(tr, this.GetType());
						tr.Close();
					}

					this.Notifications.AddRange(em.Notifications);
					this.History.AddRange(em.History);
					this.KnownDevices.AddRange(em.KnownDevices);
					this.ApiAccessAddress = em.ApiAccessAddress;

					em.Notifications.Clear();

					foreach (CustomNotification notification in Notifications)
					{
						AssignEvents(notification);
					}

                    return true;
                }
                catch //(Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public void Save ()
		{
			JsonSerializer serializer = new JsonSerializer ();
			serializer.TypeNameHandling = TypeNameHandling.Auto;
			serializer.Formatting = Formatting.Indented;
			using (TextWriter tw = new StreamWriter(BackupFileName, false)) {
				serializer.Serialize(tw, this);
				tw.Close();
			}

			if (File.Exists (FileName))
				File.Delete (FileName);

			File.Move (BackupFileName, FileName);
        } 

		private void AssignEvents (CustomNotification notification)
		{
			notification.ActiveStatusChanged += HandleActiveStatusChanged;
			notification.EnabledStatusChanged += HandleEnabledStatusChanged;

			if (notification is BlinkstickService) {
				((BlinkstickService)notification).AccessCodeChanged += HandleAccessCodeChanged;
			}
		}

		void HandleAccessCodeChanged (object sender, BlinkstickService.AccessCodeChangedEventArgs e)
		{
			client.Unsubscribe ("/devices/" + e.OldValue);
			client.Subscribe ("/devices/" + e.NewValue);
		}

		private void RemoveEvents (CustomNotification notification)
		{
			notification.ActiveStatusChanged -= HandleActiveStatusChanged;
			notification.EnabledStatusChanged -= HandleEnabledStatusChanged;
			if (notification is BlinkstickService) {
				((BlinkstickService)notification).AccessCodeChanged -= HandleAccessCodeChanged;
			}
		}

		void HandleActiveStatusChanged (object sender, EventArgs e)
		{
			LedController controller = FindControllerBySerialNumber (((CustomNotification)sender).Device);

			if (controller != null) {
				if (((CustomNotification)sender).Active)
				{
					controller.ActiveNotifications.Add((CustomNotification)sender);
				}
				else
				{
					controller.ActiveNotifications.Remove((CustomNotification)sender);
				}
			}
		}

		void HandleEnabledStatusChanged (object sender, EventArgs e)
		{
			if (sender is BlinkstickService)
			{
				if (((BlinkstickService)sender).Enabled)
				{
					client.Subscribe ("/devices/" + ((BlinkstickService)sender).AccessCode);

					if (!client.Working)
					{
						client.Connect();
					}
				}
				else
				{
					client.Unsubscribe ("/devices/" + ((BlinkstickService)sender).AccessCode);
					//Do not control the client.Disconnect() here because we first need to properly
					//unsubscribe before disconnecting. Automatically handled in the 
					//client.ChannelDisconnected event handler
				}
			}
			else if (sender is AmbiLightNotification)
			{
				if ((sender as AmbiLightNotification).Enabled)
				{
					if (!ambilightWorker.IsBusy)
					{
						ambilightWorker.RunWorkerAsync();
					}
				}
				else
				{
					if (ambilightWorker.IsBusy)
					{
						ambilightWorker.CancelAsync();
					}
				}
			}
		}

		public void Start ()
		{
			foreach (CustomNotification notification in Notifications) {
				notification.InitializeServices ();
			}

			if (monitorWorker == null) {
				monitorWorker = new BackgroundWorker ();
				monitorWorker.DoWork += new DoWorkEventHandler (monitorWorker_DoWork);
				monitorWorker.WorkerSupportsCancellation = true;
				monitorWorker.RunWorkerAsync ();
			}

			if (animationWorker == null) {
				animationWorker = new BackgroundWorker ();
				animationWorker.DoWork += new DoWorkEventHandler (animationWorker_DoWork);
				animationWorker.WorkerSupportsCancellation = true;
				animationWorker.RunWorkerAsync ();
			}

			if (ambilightWorker == null) {
				ambilightWorker = new BackgroundWorker();
				ambilightWorker.DoWork += new DoWorkEventHandler (ambilightWorker_DoWork);
				ambilightWorker.WorkerSupportsCancellation = true;
			}

			if (client == null) {
				client = new BayeuxClient (CurrentApiAccessAddress);

				client.DataReceived += (object sender, BayeuxClient.DataReceivedEventArgs e) => {
					Regex r = new Regex ("/([^/]+)$");
					Match m = r.Match (e.Channel);
					if (m.Success) {
						BlinkstickService notification = FindBlinkstickNotificaitonByAccessCode (m.Groups [1].Value);
						if (notification != null)
						{
							if (e.Data.ContainsKey ("status")) {
								if ((String)e.Data ["status"] == "off") {
									log.InfoFormat("Blinkstick device {0} turned off", notification.Name);
									notification.Active = false;
									LedController controller = FindControllerBySerialNumber(notification.Device);
									if (controller != null)
									{
										controller.MorphToColor(RgbColor.FromRgb(0, 0, 0));
									}
									return;
								}
							}

							// Handle the message
							String color = (String)e.Data ["color"];

							if (IsValidColor (color)) {
								log.InfoFormat ("New color received for Blinkstick device {0} - {1}", color, notification.Name);
								notification.Color = RgbColor.FromString (color);
								notification.Active = true;
								OnNotificationUpdated(notification);
							}
						}
						else
						{
							log.WarnFormat("Notification for access code {0} not found...", m.Groups[1].Value);
						}

					}
				};

				client.ChannelUnsubscribed += (object sender, EventArgs e) => {
					if (!client.HasSubscribedChannels) {
						client.Disconnect ();
					}
				};

				client.Disconnected += (object sender, EventArgs e) => {
					client.CloseThread();
				};
			}

			Boolean enabledDevicesExist = false;

			Boolean ambilightExist = false;

			foreach (CustomNotification notification in Notifications) {
				if (notification is BlinkstickService) {
					if (notification.Enabled) {
						client.Subscribe ("/devices/" + ((BlinkstickService)notification).AccessCode);
						enabledDevicesExist = true;
					}
				}
				else if (notification is AmbiLightNotification && notification.Enabled)
				{
					ambilightExist = true;
				}

			}

			if (enabledDevicesExist) {
				client.Connect ();
			}

			if (ambilightExist)
				ambilightWorker.RunWorkerAsync();
		}

		void ScreenCapture(Object statInfo)
		{
		}

		private Boolean IsValidColor (String color)
		{
			return Regex.IsMatch(color, "^#[A-Ha-h0-9]{6}$");
		}


		public void Stop ()
		{
			if (monitorWorker != null) {
				monitorWorker.CancelAsync ();
				monitorWorker = null;
			}

			if (animationWorker != null) {
				animationWorker.CancelAsync ();
				animationWorker = null;
			}

			if (ambilightWorker != null) {
				ambilightWorker.CancelAsync ();
				ambilightWorker = null;
			}

			foreach (CustomNotification notification in Notifications) {
				notification.FinalizeServices ();
			}

			if (client.Working) {
				client.Disconnect();
			}
		}

		void ambilightWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			log.Info("Starting ambilight");

			AbstractScreenCapture capture = null;

			if (BlinkstickDeviceFinder.IsUnix())
			{
				capture = new LinuxScreenCapture();
			}
			else
			{
				capture = new WindowsScreenCapture();
			}

			BackgroundWorker worker = (BackgroundWorker)sender;

			LedController ambilightController = null;
				
			foreach (CustomNotification notification in Notifications) {
				if (notification is AmbiLightNotification)
					ambilightController = FindControllerBySerialNumber(notification.Device);
			}

			while (!worker.CancellationPending) {
				byte r, g, b;
				if (capture.ScreenCap(out r, out g, out b))
				{
					//ambilightController.MorphToColor(RgbColor.FromRgb(r, g, b));
					ambilightController.SendColor(r, g, b);
				}
				//log.DebugFormat("Cap {0:x2} {1:x2} {2:x2}", r, g, b);

				//Thread.Sleep(100);
			}

			log.Info("Closing ambilight");
		}

		void animationWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			log.Info("Starting animator");

			BackgroundWorker worker = (BackgroundWorker)sender;
			while (!worker.CancellationPending) {
				foreach (LedController controller in Controllers)
				{
					if (controller.Animating)
						continue;

					if (controller.CanExecuteEvent())
					{
						if (!controller.LastNotificationTimestamp.HasValue)
						{
							controller.LastNotificationTimestamp = DateTime.Now;
						}

						//when delay time has passed
						if (DateTime.Now - controller.LastNotificationTimestamp.Value > 
						    TimeSpan.FromMilliseconds(DelayBetweenEvents))
						{
							controller.NextNotification();
							controller.ExecuteEvent();
							controller.LastNotificationTimestamp = null;
						}
					}
				}

				Thread.Sleep (100);
			}

			log.Info("Closing animator");
		}

        void monitorWorker_DoWork (object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;
			Running = true;

			log.Info("Starting monitor");

			while (!worker.CancellationPending) {
				int eventCount = 0;

				eventCount = Notifications.Count;

				for (int i = eventCount - 1; i >= 0; i--)
				{
					try
					{
						CustomNotification ev = Notifications[i];

						if (!ev.GetSupportsPeriodicChecking() || !ev.Enabled)
							continue;
						
						if (ev.LastChecked.AddSeconds(ev.GetCheckFrequency()) > DateTime.Now) {
							continue;
						}

						ev.LastChecked = DateTime.Now;

						Boolean active = ev.Check();
						Boolean previousActive = ev.Active;
						//Boolean active = false;

						lock(ev)
						{
							ev.Active = active;
						}

						if (active != previousActive)
						{
							OnNotificationUpdated(ev);
						}
					}

					catch (Exception ex)
					{
						if (ex.Message != "")
						{
							log.ErrorFormat(ex.Message);
						}
					}

					if (worker.CancellationPending)
						break;
				}

				Thread.Sleep (500);
			}

			Running = false;

			log.Info("Monitor exiting");
		}

		#region Animation Thread Helpers
        void WaitSeconds(int seconds)
        {
            DateTime start = DateTime.Now;
            while (animationWorker != null && !animationWorker.CancellationPending && 
			       start.AddSeconds(seconds) > DateTime.Now)
            {
                Thread.Sleep(10);
            }
        }

        void WaitMiliseconds(int miliseconds)
        {
            DateTime start = DateTime.Now;
            while (animationWorker != null && !animationWorker.CancellationPending && 
			       start.AddMilliseconds(miliseconds) > DateTime.Now)
            {
                Thread.Sleep(10);
            }
        }
		#endregion

		public HistoryItem AddHistoryItemForNotification(CustomNotification notification, String text)
		{
			HistoryItem item = new HistoryItem(notification.Name, text);
			History.Add (item);
			return item;
		}

		public void AddNotification(CustomNotification notification)
		{
			Notifications.Add(notification);
			AssignEvents(notification);

			if (notification is BlinkstickService) {
				if (notification.Enabled) {
					client.Subscribe ("/devices/" + ((BlinkstickService)notification).AccessCode);
					if (!client.Working)
					{
						client.Connect ();
					}
				}
			}
		}

		public void RemoveNotification (CustomNotification notification)
		{
			Notifications.Remove (notification);
			RemoveEvents (notification);
			foreach (LedController controller in Controllers) {
				if (controller.ActiveNotifications.Contains(notification))
				{
					controller.ActiveNotifications.Remove(notification);
					return;
				}
			}
		}

		public List<DeviceEntity> GetDeviceList ()
		{
			List<DeviceEntity> devices = new List<DeviceEntity> ();

			foreach (LedController controller in Controllers) {
				devices.Add (new DeviceEntity (controller.Device.Serial, controller.Device.Name, true));
			}

			foreach (DeviceEntity device in KnownDevices) {
				if (FindControllerBySerialNumber(device.Serial) == null)
				{
					devices.Add(new DeviceEntity(device.Serial, device.Name, false));
				}
			}

			devices.Sort(delegate(DeviceEntity d1, DeviceEntity d2) {
				return d1.ToString().CompareTo(d2.ToString());
			});

			return devices;
		}

		public List<DeviceEntity> GetOnlineDeviceList ()
		{
			List<DeviceEntity> devices = new List<DeviceEntity> ();

			foreach (LedController controller in Controllers) {
				devices.Add (new DeviceEntity (controller.Device.Serial, controller.Device.Name, true));
			}

			devices.Sort(delegate(DeviceEntity d1, DeviceEntity d2) {
				return d1.ToString().CompareTo(d2.ToString());
			});

			return devices;
		}
	}
}

