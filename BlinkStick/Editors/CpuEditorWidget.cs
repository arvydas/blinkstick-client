using System;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class CpuEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationCpu Notification;

        public CpuEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(Notification notification)
        {
            this.Notification = (NotificationCpu)notification;

            radiobuttonMonitor.Active = this.Notification.Configuration == NotificationCpu.ConfigurationEnum.Monitor;
            radiobuttonAlert.Active = this.Notification.Configuration == NotificationCpu.ConfigurationEnum.Alert;
            comboboxTriggerType.Active = this.Notification.TriggerType == NotificationCpu.TriggerTypeEnum.More ? 0 : 1;
            spinbuttonCheckPeriod.Value = this.Notification.CheckPeriod;
            spinbuttonCpuPercent.Value = this.Notification.AlertPercent;

            UpdateUI();
        }

        public bool IsValid()
        {
            return true;
        }

        public void UpdateNotification()
        {
            this.Notification.Configuration = radiobuttonMonitor.Active ? 
                NotificationCpu.ConfigurationEnum.Monitor : NotificationCpu.ConfigurationEnum.Alert;
            this.Notification.TriggerType = comboboxTriggerType.Active == 0 ? 
                NotificationCpu.TriggerTypeEnum.More : NotificationCpu.TriggerTypeEnum.Less;
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

