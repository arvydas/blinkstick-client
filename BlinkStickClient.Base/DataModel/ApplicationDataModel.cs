using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using BlinkStickDotNet;

namespace BlinkStickClient.DataModel
{
    public class ApplicationDataModel
    {
        public event EventHandler<EventArgs> PatternsUpdated;

        public void OnPatternsUpdated()
        {
            if (PatternsUpdated != null)
            {
                PatternsUpdated(this, EventArgs.Empty);
            }
        }

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

        private static String _ApplicationVersion = "";
        public static String ApplicationVersion {
            get {
                if (_ApplicationVersion == "")
                {
                    _ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
                    _ApplicationVersion = _ApplicationVersion.Substring (0, _ApplicationVersion.LastIndexOf ('.'));
                }
                return _ApplicationVersion;
            }
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
        public Boolean AddIfDoesNotExist(BlinkStick led)
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

        #region Naming helpers
        public String GetPatternName(string baseName = "Pattern", int start = 1)
        {
            Regex r = new Regex(@"(\d+)$");
            Match m = r.Match(baseName);

            if (m.Success)
            {
                start = Convert.ToInt32(m.Groups[0].Value);
                baseName = baseName.Substring(0, m.Groups[0].Index);
            }

            String name = "";
            int i = start;
            Boolean found;

            do
            {
                found = false;
                name = String.Format("{0}{1}", baseName, i);

                foreach (Pattern p in Patterns)
                {
                    if (p.Name == name)
                    {
                        i++;
                        name = "";
                        found = true;
                    }
                }
            } 
            while (found && i < 100);

            return name;
        }

        public String GetNotificationName(string baseName, int start = 1)
        {
            Regex r = new Regex(@"(\d+)$");
            Match m = r.Match(baseName);

            if (m.Success)
            {
                start = Convert.ToInt32(m.Groups[0].Value);
                baseName = baseName.Substring(0, m.Groups[0].Index);
            }

            String name = "";
            int i = start;
            Boolean found;

            do
            {
                found = false;
                name = String.Format("{0}{1}", baseName, i);

                foreach (Notification n in Notifications)
                {
                    if (n.Name == name)
                    {
                        i++;
                        name = "";
                        found = true;
                    }
                }
            } 
            while (found && i < 100);

            return name;
        }
        #endregion
    }
}

