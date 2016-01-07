using System;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;

namespace BlinkStickClient
{
    public static class Logger
    {
        public static void Setup(String logFile, String logLevel)
        {
            Stop();

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Clear();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = false;
            roller.File = logFile;
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "10MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            ConsoleAppender console = new ConsoleAppender();
            console.Layout = patternLayout;
            hierarchy.Root.AddAppender(console);

            /*
            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);
            */

            if (logLevel == "Full")
            {
                hierarchy.Root.Level = Level.Debug;
                hierarchy.Configured = true;
            }
            else if (logLevel == "Light")
            {
                hierarchy.Root.Level = Level.Info;
                hierarchy.Configured = true;
            }
            else
            {
                hierarchy.Root.Level = Level.Off;
                hierarchy.Configured = false;
            }
        }

        public static void Stop()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Shutdown();
        }
    }
}

