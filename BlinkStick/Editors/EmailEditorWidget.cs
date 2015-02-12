using System;
using System.ComponentModel;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using System.Security.Authentication;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class EmailEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationImap Notification;

        private Boolean IgnoreChanges = false;

        public EmailEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.Notification notification)
        {
            Notification = (notification as NotificationImap);

            IgnoreChanges = true;

            entryUsername.Text = Notification.Username;
            entryPassword.Text = Notification.Password;
            entryServer.Text = Notification.ServerAddress;
            spinbuttonPort.Value = Notification.Port;
            checkbuttonUseSSL.Active = Notification.UseSsl;
            spinbuttonCheckPeriod.Value = Notification.CheckPeriod;

            UpdateUI();

            IgnoreChanges = false;

            if (spinbuttonPort.Value == 0)
            {
                OnCheckbuttonUseSSLToggled(checkbuttonUseSSL, EventArgs.Empty);
            }
        }

        public bool IsValid(Gtk.Window window)
        {
            if (this.entryUsername.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter Email", Gtk.MessageType.Error);
                return false;
            }

            if (this.entryPassword.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter password", Gtk.MessageType.Error);
                return false;
            }

            if (this.entryServer.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter server address", Gtk.MessageType.Error);
                return false;
            }

            return true;
        }

        public void UpdateNotification()
        {
            Notification.Username = entryUsername.Text;
            Notification.Password = entryPassword.Text;
            Notification.CheckPeriod = spinbuttonCheckPeriod.ValueAsInt;
            Notification.ServerAddress = entryServer.Text;
            Notification.Port = spinbuttonPort.ValueAsInt;
            Notification.UseSsl = checkbuttonUseSSL.Active;
        }
        #endregion

        protected void OnButtonRefreshClicked (object sender, EventArgs e)
        {
            if (this.entryUsername.Text.Trim() == "")
            {
                labelCurrentValue.Text = "Value: (Username missing)";
                return;
            }

            if (this.entryPassword.Text.Trim() == "")
            {
                labelCurrentValue.Text = "Value: (Password missing)";
                return;
            }

            if (this.entryServer.Text.Trim() == "")
            {
                labelCurrentValue.Text = "Value: (Server missing)";
                return;
            }

            labelCurrentValue.Text = "Value: Checking...";

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object s, DoWorkEventArgs ea) => {
                String value;
                try
                {
                    value = String.Format("Value: {0} unread", 
                        this.Notification.GetValue(
                            this.entryUsername.Text, 
                            this.entryPassword.Text, 
                            this.entryServer.Text,
                            this.spinbuttonPort.ValueAsInt,
                            this.checkbuttonUseSSL.Active
                        ));
                }
                catch (Exception ex)
                {
                    value = String.Format("Value: {0}", ex.Message);
                }

                Gtk.Application.Invoke(delegate {
                    labelCurrentValue.Text = value;
                });
            };
            worker.RunWorkerAsync();
        }

        private void UpdateUI()
        {
            if (checkbuttonUseSSL.Active)
            {
                labelPortInfo.Markup = "<i>Default: 993</i>";
            }
            else
            {
                labelPortInfo.Markup = "<i>Default: 143</i>";
            }
        }

        protected void OnCheckbuttonUseSSLToggled (object sender, EventArgs e)
        {
            if (IgnoreChanges)
                return;

            if (checkbuttonUseSSL.Active)
            {
                spinbuttonPort.Value = 993;
            }
            else
            {
                spinbuttonPort.Value = 143;
            }

            UpdateUI();
        }
    }
}

