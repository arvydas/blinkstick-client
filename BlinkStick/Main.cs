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
using Gtk;
using System.IO;
using log4net;

namespace BlinkStickClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            if (args.Length > 0 && args[0] == "--ambilight")
            {
                AmbilightWindowsService service = new AmbilightWindowsService();
                service.Run();
                return;
            }

            string logFileConfigPath = System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
			FileInfo finfo = new FileInfo(logFileConfigPath);
			log4net.Config.XmlConfigurator.ConfigureAndWatch(finfo); 
			
			//Create log files in the application data folder
			if (!Directory.Exists(MainWindow.LogFolder))
			{
				Directory.CreateDirectory(MainWindow.LogFolder);
			}
			
			//Update file appender to log to the correct location
			foreach (log4net.Appender.IAppender appender in log4net.LogManager.GetRepository().GetAppenders())
			{
				if (appender is log4net.Appender.FileAppender)
				{
					((log4net.Appender.FileAppender)appender).File = MainWindow.LogFile;
					((log4net.Appender.FileAppender)appender).ActivateOptions();
					
					break;
				}
			}

			ILog log = LogManager.GetLogger("Main");	

			log.Info("--------------------------------------");
			log.InfoFormat("BlinkStick Client {0} application started", MainWindow.ApplicationVersion);

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

			Application.Init ();
            //Gtk.Rc.Parse(Path.Combine(MainWindow.ExecutableFolder, "Theme", "gtkrc")); 
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();

            HidSharp.HidDeviceLoader.FreeUsbResources();
		}
	}
}
