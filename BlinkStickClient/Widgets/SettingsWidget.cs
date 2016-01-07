using System;
using System.IO;
using BlinkStickClient.Classes;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SettingsWidget : Gtk.Bin
    {
        ApplicationSettings ApplicationSettings;

        Boolean IgnoreControlChanges = true;

        public SettingsWidget()
        {
            this.Build();
        }

        public void LoadSettings(ApplicationSettings applicationSettings)
        {
            ApplicationSettings = applicationSettings;

            int i = -1;

            foreach (String folder in Directory.GetDirectories(System.IO.Path.Combine(MainWindow.ExecutableFolder, "Theme")))
            {
                if (File.Exists(System.IO.Path.Combine(folder, "gtk-2.0", "gtkrc")))
                {
                    String themeName = System.IO.Path.GetFileName(folder);

                    comboboxTheme.AppendText(themeName);
                    i++;

                    if (ApplicationSettings.Theme == themeName)
                    {
                        comboboxTheme.Active = i;
                    }
                }
            }

            if (ApplicationSettings.LogLevel == "Full")
            {
                comboboxLogging.Active = 2;
            }
            else if (ApplicationSettings.LogLevel == "Light")
            {
                comboboxLogging.Active = 1;
            }
            else
            {
                comboboxLogging.Active = 0;
            }

            IgnoreControlChanges = false;
        }

        protected void OnComboboxThemeChanged(object sender, EventArgs e)
        {
            if (IgnoreControlChanges)
                return;

            ApplicationSettings.Theme = comboboxTheme.ActiveText;
            ApplicationSettings.Save();

            labelRestartWarning.Visible = true;
        }

        protected void OnComboboxLoggingChanged(object sender, EventArgs e)
        {
            if (IgnoreControlChanges)
                return;

            ApplicationSettings.LogLevel = comboboxLogging.ActiveText;
            ApplicationSettings.Save();

            labelRestartWarning.Visible = true;
        }

        protected void OnButtonOpenLogFolderClicked(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(MainWindow.LogFolder))
            {
                try
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = MainWindow.LogFolder,
                        UseShellExecute = true,
                        Verb = "open"
                    }
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(MainWindow.Instance, "Unable to open log folder: " + ex.Message, Gtk.MessageType.Error);
                }
            }
            else
            {
                MessageBox.Show(MainWindow.Instance, "Log folder does not exist.\r\n\r\nPlease enabled logging before accessing log folder.", Gtk.MessageType.Error);
            }
        }

        protected void OnCheckbuttonStartupToggled(object sender, EventArgs e)
        {
            if (IgnoreControlChanges)
                return;

            ApplicationSettings.StartWithWindows = (sender as Gtk.CheckButton).Active;
            MainWindow.RegisterStartup(ApplicationSettings.StartWithWindows);
            ApplicationSettings.Save();
        }
    }
}

