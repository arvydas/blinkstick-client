using System;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using BlinkStickClient.DataModel;
using System.IO;

namespace BlinkStickClient.Classes
{
    public class ApplicationSettings
    {
        public Boolean SingleBlinkStickMode = false;
        public String ApplicationTitle = "";
        public Boolean AllowModeChange = true;

        public String Theme = "Clearlooks";
        public String LogLevel = "Off";

        public Boolean StartWithWindows = true;

        private String FileName;
        private String BackupFileName;

        public ApplicationSettings()
        {
            FileName = Path.Combine (ApplicationDataModel.DefaultSettingsFolder, "settings.json");
            BackupFileName = Path.Combine (ApplicationDataModel.DefaultSettingsFolder, "settings.~json");            
        }

        #region Load/Save functions
        public void Load()
        {
            if (!Load(FileName))
            {
                Load(BackupFileName);
            }

            if (!File.Exists(FileName))
            {
                Save();
            }
        }

        private Boolean Load(String SettingsFileName)
        {
            if (File.Exists(SettingsFileName))
            {
                try
                {
                    ApplicationSettings settings;

                    JsonSerializer serializer = new JsonSerializer ();
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    serializer.Formatting = Formatting.Indented;
                    using (TextReader tr = new StreamReader(SettingsFileName)) {
                        settings = (ApplicationSettings)serializer.Deserialize(tr, this.GetType());
                        tr.Close();
                    }

                    this.Assign(settings);

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
        #endregion


        #region Assignment
        public void Assign(ApplicationSettings settings)
        {
            this.ApplicationTitle = settings.ApplicationTitle;
            this.SingleBlinkStickMode = settings.SingleBlinkStickMode;
            this.AllowModeChange = settings.AllowModeChange;
            this.Theme = settings.Theme;
            this.LogLevel = settings.LogLevel;
            this.StartWithWindows = settings.StartWithWindows;
        }
        #endregion

    }
}

