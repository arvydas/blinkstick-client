using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Classes;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class OverviewSingleWidget : Gtk.Bin
    {
        public ApplicationDataModel DataModel;
        public ApplicationSettings ApplicationSettings;

        private global::BlinkStickClient.BlinkStickInfoWidget blinkstickinfowidget2;

        public OverviewSingleWidget()
        {
            this.Build();

            // Container child vbox3.Gtk.Box+BoxChild
            this.blinkstickinfowidget2 = new global::BlinkStickClient.BlinkStickInfoWidget ();
            this.blinkstickinfowidget2.Events = ((global::Gdk.EventMask)(256));
            this.blinkstickinfowidget2.Name = "blinkstickinfowidget2";
            this.vbox3.Add (this.blinkstickinfowidget2);
            global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.blinkstickinfowidget2]));
            w7.Position = 1;
            w7.Expand = false;
            w7.Fill = false;
        }

        public void UpdateUI()
        {
            buttonConfigure.Sensitive = DataModel.Devices.Count > 0 && DataModel.Devices[0].Led != null;
            blinkstickinfowidget2.UpdateUI(DataModel.Devices[0]);
        }

        protected void OnButtonRefreshClicked(object sender, EventArgs e)
        {
            UpdateUI();
        }

        protected void OnButtonConfigureClicked(object sender, EventArgs e)
        {
            ConfigureBlinkStickDialog dialog = new ConfigureBlinkStickDialog();
            dialog.DeviceSettings = DataModel.Devices[0];
            dialog.ApplicationSettings = this.ApplicationSettings;
            dialog.UpdateUI();
            dialog.Run();
            dialog.Destroy();
        }
    }
}

