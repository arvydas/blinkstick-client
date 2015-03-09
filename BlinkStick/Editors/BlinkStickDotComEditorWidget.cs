using System;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class BlinkStickDotComEditorWidget : Gtk.Bin, IEditorInterface
    {
        private static Gdk.Atom _atom = Gdk.Atom.Intern("CLIPBOARD", false);
        private Gtk.Clipboard _clipBoard = Gtk.Clipboard.Get(_atom);

        public NotificationBlinkStickDotCom Notification;

        public BlinkStickDotComEditorWidget()
        {
            this.Build();
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.Notification notification)
        {
            this.Notification = (NotificationBlinkStickDotCom)notification;

            entryAccessCode.Text = this.Notification.AccessCode;
        }

        public bool IsValid(Gtk.Window window)
        {
            if (entryAccessCode.Text.Trim() == "")
            {
                MessageBox.Show(window, "Please enter access code", Gtk.MessageType.Error);
                return false;
            }

            return true;
        }

        public void UpdateNotification()
        {
            this.Notification.AccessCode = entryAccessCode.Text;
        }
        #endregion

        protected void OnButtonPasteClicked (object sender, EventArgs e)
        {
            entryAccessCode.Text = _clipBoard.WaitForText();
        }

        protected void OnButtonRegisterDeviceClicked (object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.blinkstick.com/devices/new");
            }
            catch
            {
            }
        }
    }
}

