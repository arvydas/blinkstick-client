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
using System.Linq;
using Gtk;
using System.IO;
using log4net;
using BlinkStickClient.Utils;
using BlinkStickClient.DataModel;
using System.Reflection;
using System.Diagnostics;
using BlinkStickClient.Classes;

namespace BlinkStickClient
{
	class MainClass
	{
        public static ILog log;

		public static int Main (string[] args)
		{
            String version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
            String versionName = version.Substring (0, version.LastIndexOf ('.'));
            versionName = versionName.Substring (0, versionName.LastIndexOf ('.'));

            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(true);
            object configRaw = attributes.FirstOrDefault(a => a.GetType() == typeof(AssemblyConfigurationAttribute));
            if (configRaw != null) {
                AssemblyConfigurationAttribute config = (AssemblyConfigurationAttribute)configRaw;
                if (config.Configuration != "")
                {
                    versionName += "-" + config.Configuration;
                }
            }

            ApplicationDataModel.ApplicationVersion = versionName;

            if (args.Length > 1 && args[0] == "--build-config")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(args[1]);
                file.WriteLine(String.Format("#define AppVersion \"{0}\"\r\n#define AppFullVersion \"{1}\"", versionName, version));
                file.Close();
                return 0;
            }

            ApplicationSettings applicationSettings = new ApplicationSettings();
            applicationSettings.Load();

            if (args.Length > 0 && args[0] == "--register")
            {
                MainWindow.RegisterStartup(true);

                applicationSettings.StartWithWindows = true;
                applicationSettings.Save();
                return 0;
            }
            else if (args.Length > 0 && args[0] == "--unregister")
            {
                MainWindow.RegisterStartup(false);

                applicationSettings.StartWithWindows = false;
                applicationSettings.Save();
                return 0;
            }

            Boolean ambilightMode = args.Length > 0 && args[0] == "--ambilight";

            if (ambilightMode)
            {
                Logger.Setup(Path.Combine(MainWindow.LogFolder, "ambilight.log"), applicationSettings.LogLevel);
            }
            else
            {
                Logger.Setup(MainWindow.LogFile, applicationSettings.LogLevel);
            }

            log = LogManager.GetLogger("Main");    

            log.Info("--------------------------------------");
            log.InfoFormat("BlinkStick Client {0} application started", ApplicationDataModel.ApplicationVersion);

            if (ambilightMode)
            {
                AmbilightWindowsService service = new AmbilightWindowsService();
                service.Run();
                Logger.Stop();
                return 0;
            }

            #if !DEBUG
			GLib.ExceptionManager.UnhandledException += delegate(GLib.UnhandledExceptionArgs args2) {
				Exception e = (Exception)args2.ExceptionObject;
				log.Fatal("Unhandled exception occured:");
				if (e.InnerException != null)
				{
					log.FatalFormat("Inner Exception: {0}", e.InnerException.Message);
					log.Fatal(e.InnerException.StackTrace);
				}
				else
				{
					log.Fatal(e.Message);
					log.Fatal(e.StackTrace);
				}

				args2.ExitApplication = true;

				BlinkStickClient.Utils.MessageBox.Show (null, "Unfortunately BlinkStick Client has crashed :(\n\r" +
				                                  "The details about the crash are available in the log file: \r\n\r\n" +
				                                  MainWindow.LogFile + "\r\n\r\n" +
				                                  "The application will now close.", MessageType.Error);
			};
            #endif

            //TODO: Check if platform is Windows
            if (!CheckWindowsGtk ())
                return 1;

            Environment.SetEnvironmentVariable("GTK2_RC_FILES", Path.Combine(MainWindow.ExecutableFolder, "Theme", applicationSettings.Theme, "gtk-2.0", "gtkrc"));

            Application.Init ();
            MainWindow win = new MainWindow ();
            win.ApplicationSettings = applicationSettings;
            win.LoadEverything();
            if (!(args.Length > 0 && args[0] == "--tray"))
            {
                win.Show ();
            }
			Application.Run ();

            HidSharp.HidDeviceLoader.FreeUsbResources();
            Logger.Stop();

            return 0;
		}

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetDllDirectory (string lpPathName);

        static bool CheckWindowsGtk ()
        {
            string location = null;
            Version version = null;
            Version minVersion = new Version (2, 12, 22);
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\InstallFolder")) {
                if (key != null)
                    location = key.GetValue (null) as string;
            }
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Xamarin\GtkSharp\Version")) {
                if (key != null)
                    Version.TryParse (key.GetValue (null) as string, out version);
            }
            //TODO: check build version of GTK# dlls in GAC
            if (version == null || version < minVersion || location == null || !File.Exists (Path.Combine (location, "bin", "libgtk-win32-2.0-0.dll"))) {
                log.Error ("Did not find required GTK# installation");
                string url = "http://monodevelop.com/Download";
                string caption = "Fatal Error";
                string message =
                    "{0} did not find the required version of GTK#. Please click OK to open the download page, where " +
                    "you can download and install the latest version.";
                if (DisplayWindowsOkCancelMessage (
                    string.Format (message, "BlinkStick Client " + ApplicationDataModel.ApplicationVersion, url), caption)
                ) {
                    Process.Start (url);
                }
                return false;
            }
            log.Info ("Found GTK# version " + version);
            var path = Path.Combine (location, @"bin");
            try {
                if (SetDllDirectory (path)) {
                    return true;
                }
            } catch (EntryPointNotFoundException) {
            }
            // this shouldn't happen unless something is weird in Windows
            log.Error ("Unable to set GTK+ dll directory");
            return true;
        }

        static bool DisplayWindowsOkCancelMessage (string message, string caption)
        {
            var name = typeof(int).Assembly.FullName.Replace ("mscorlib", "System.Windows.Forms");
            var asm = Assembly.Load (name);
            var md = asm.GetType ("System.Windows.Forms.MessageBox");
            var mbb = asm.GetType ("System.Windows.Forms.MessageBoxButtons");
            var okCancel = Enum.ToObject (mbb, 1);
            var dr = asm.GetType ("System.Windows.Forms.DialogResult");
            var ok = Enum.ToObject (dr, 1);
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
            return md.InvokeMember ("Show", flags, null, null, new object[] { message, caption, okCancel }).Equals (ok);
        }
	}
}
