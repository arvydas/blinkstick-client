using System;
using Microsoft.Win32;

namespace BlinkStickClient
{
    static class StartupManager
    {
        #region Startup Registration Procedure
        public static void RegisterStartup(Boolean Install, String tag, String path, String param = "")
        {
            // get reference to the HKLM registry key...
            RegistryKey rkHKCU = Registry.CurrentUser;
            RegistryKey rkRun;

            // get reference to Software\Microsoft\Windows\CurrentVersion\Run subkey
            // with permission to write to it...
            try
            {
                rkRun = rkHKCU.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            }
            catch
            {
                // close the HKCU key...
                rkHKCU.Close();
                return;
            }

            // do as told...
            if (Install)
            {
                // install the application...
                try
                {
                    // create a value with name same as the data...
                    rkRun.SetValue(tag, "\"" + path + "\"" + (param != "" ? param : ""));
                }
                catch
                {
                }
            }
            else
            {
                // uninstall the application
                try
                {
                    // delete the application's value...
                    rkRun.DeleteValue(tag);
                }
                catch
                {
                }
            }

            // close the subkey...
            rkRun.Close();
            // close the HKLM key...
            rkHKCU.Close();
        }
        #endregion
    }

}

