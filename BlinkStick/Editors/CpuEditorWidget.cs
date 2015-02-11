using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class CpuEditorWidget : Gtk.Bin, IEditorInterface
    {
        private HardwareNotification Notification;

        public CpuEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(Notification notification)
        {
            this.Notification = (HardwareNotification)notification;

            radiobuttonMonitor.Active = this.Notification.Configuration == HardwareNotification.ConfigurationEnum.Monitor;
            radiobuttonAlert.Active = this.Notification.Configuration == HardwareNotification.ConfigurationEnum.Alert;
            comboboxTriggerType.Active = this.Notification.TriggerType == HardwareNotification.TriggerTypeEnum.More ? 0 : 1;
            spinbuttonCheckPeriod.Value = this.Notification.CheckPeriod;
            spinbuttonCpuPercent.Value = this.Notification.AlertPercent;

            GtkLabel2.Markup = String.Format("<b>Configure {0} monitoring</b>", this.Notification.GetTypeName());

            UpdateUI();
        }

        public bool IsValid(Gtk.Window window)
        {
            return true;
        }

        public void UpdateNotification()
        {
            this.Notification.Configuration = radiobuttonMonitor.Active ? 
                HardwareNotification.ConfigurationEnum.Monitor : HardwareNotification.ConfigurationEnum.Alert;
            this.Notification.TriggerType = comboboxTriggerType.Active == 0 ? 
                HardwareNotification.TriggerTypeEnum.More : HardwareNotification.TriggerTypeEnum.Less;
            this.Notification.CheckPeriod = spinbuttonCheckPeriod.ValueAsInt;
            this.Notification.AlertPercent = spinbuttonCpuPercent.ValueAsInt;
        }

        #endregion

        public void UpdateUI()
        {
            comboboxTriggerType.Sensitive = radiobuttonAlert.Active;
            spinbuttonCheckPeriod.Sensitive = radiobuttonAlert.Active;
            spinbuttonCpuPercent.Sensitive = radiobuttonAlert.Active;
        }

        protected void OnRadiobuttonAlertToggled (object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}

