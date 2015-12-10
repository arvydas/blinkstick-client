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

        public OverviewSingleWidget()
        {
            this.Build();
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

