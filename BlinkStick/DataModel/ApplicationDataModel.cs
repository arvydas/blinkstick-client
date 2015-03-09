using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class ApplicationDataModel
    {
        public List<Pattern> Patterns = new List<Pattern>();
        public ObservableCollectionEx<Notification> Notifications = new ObservableCollectionEx<Notification>();
        public List<BlinkStickDeviceSettings> Devices = new List<BlinkStickDeviceSettings>();
        public ObservableCollection<TriggeredEvent> TriggeredEvents = new ObservableCollection<TriggeredEvent>();

        private String FileName;
        private String BackupFileName;

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

        public String ApiAccessAddress {
            get;
            set;
        }

        private const String DefaultApiAccessAddress = "http://live.blinkstick.com:9292/faye";

        public String CurrentApiAccessAddress {
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

        public ApplicationDataModel()
        {
            FileName = Path.Combine (DefaultSettingsFolder, "data.json");
            BackupFileName = Path.Combine (DefaultSettingsFolder, "data.~json");
        }


        #region Load/Save functions
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
                    ApplicationDataModel data;

                    JsonSerializer serializer = new JsonSerializer ();
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    serializer.Formatting = Formatting.Indented;
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    using (TextReader tr = new StreamReader(SettingsFileName)) {
                        data = (ApplicationDataModel)serializer.Deserialize(tr, this.GetType());
                        tr.Close();
                    }

                    this.Assign(data);

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
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            using (TextWriter tw = new StreamWriter(BackupFileName, false)) {
                serializer.Serialize(tw, this);
                tw.Close();
            }

            if (File.Exists (FileName))
                File.Delete (FileName);

            File.Move (BackupFileName, FileName);
        } 
        #endregion

        #region Helpers
        public Boolean PatternNameExists(Pattern current, String name)
        {
            foreach (Pattern pattern in this.Patterns)
            {
                if (pattern != current && pattern.Name == name)
                    return true;
            }

            return false;
        }
        #endregion

        #region Assignment
        public void Assign(ApplicationDataModel data)
        {
            this.Patterns.Clear();
            this.Patterns.AddRange(data.Patterns);

            this.Notifications.Clear();
            foreach (Notification n in data.Notifications)
            {
                this.Notifications.Add(n);
            }

            this.Devices.Clear();
            this.Devices.AddRange(data.Devices);

            this.TriggeredEvents.Clear();
            foreach (TriggeredEvent ev in data.TriggeredEvents)
            {
                this.TriggeredEvents.Add(ev);
            }
        }
        #endregion

        #region Device list helpers
        public Boolean AddIfDoesNotExist(BlinkStickDotNet.BlinkStick led)
        {
            Boolean newRecord = true;

            foreach (BlinkStickDeviceSettings current in Devices)
            {
                if (current.Serial == led.Serial)
                {
                    current.Touched = true;
                    if (current.Led == null)
                    {
                        current.Led = led;
                        current.Led.OpenDevice();
                    }
                    newRecord = false;
                }
            }

            if (newRecord)
            {
                BlinkStickDeviceSettings settings = new BlinkStickDeviceSettings(led);
                settings.Touched = true;
                Devices.Add(settings);
                led.OpenDevice();
            }

            return newRecord;
        }

        public BlinkStickDeviceSettings FindBySerial(String serial)
        {
            foreach (BlinkStickDeviceSettings current in Devices)
            {
                if (current.Serial == serial)
                {
                    return current;
                }
            }

            return null;
        }

        public void Untouch()
        {
            foreach (BlinkStickDeviceSettings settings in Devices)
            {
                settings.Touched = false;
            }
        }

        public void ProcessUntouched()
        {
            foreach (BlinkStickDeviceSettings settings in Devices)
            {
                if (!settings.Touched)
                {
                    settings.Led = null;
                }
            }
        }
        #endregion
    }
}

