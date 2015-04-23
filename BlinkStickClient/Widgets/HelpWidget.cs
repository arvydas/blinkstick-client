using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class HelpWidget : Gtk.Bin
    {
        public HelpWidget()
        {
            this.Build();

            this.imageLogo.File = global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "logo.png");

            this.labelCopyright.Text = "Copyright (c) Agile Innovative Ltd 2013-" + DateTime.Now.Year.ToString();
            this.labelVersionInfo.LabelProp = String.Format("Version <b>{0}</b>", ApplicationDataModel.ApplicationVersion);
        }

        protected void OnButtonBlinkStickDotComLinkClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.blinkstick.com");
            }
            catch
            {
                MessageBox.Show(null, "Unable to open url http://www.blinkstick.com", Gtk.MessageType.Error);
            }        
        }

        protected void OnButtonSupportForumsClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://forums.blinkstick.com");
            }
            catch
            {
                MessageBox.Show(null, "Unable to open url https://forums.blinkstick.com", Gtk.MessageType.Error);
            }        
        }

        protected void OnButtonSupportEmailClicked(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:support@blinkstick.com");
            }
            catch
            {
                MessageBox.Show(null, "Unable to open default email program to send a message to support@blinkstick.com", Gtk.MessageType.Error);
            }
        }
    }
}

