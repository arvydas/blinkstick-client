using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace BlinkStickClient.DataModel
{
    public class ApplicationDataModel
    {
        public List<Pattern> Patterns = new List<Pattern>();

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
        }
        #endregion
    }
}

