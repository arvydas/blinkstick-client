using System;
using System.Collections.Generic;
using BlinkStickClient.DataModel;
using BlinkStickClient.Utils;
using System.Windows.Forms;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class KeyboardEditorWidget : Gtk.Bin, IEditorInterface
    {
        private NotificationKeyboard Notification;

        private List<String> KeysString = new List<String>();
        private List<Keys> AvailableKeys = new List<Keys>();

        public KeyboardEditorWidget()
        {
            this.Build();

            foreach (Keys key in (Keys[]) Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Escape || key == Keys.Enter || key >= Keys.Space && key <= Keys.Help || 
                    key >= Keys.A && key <= Keys.Z || key >= Keys.NumPad0 && key <= Keys.F24)
                {
                    if (!this.KeysString.Contains(key.ToString()))
                    {
                        this.KeysString.Add(key.ToString());
                        comboboxKey.AppendText(key.ToString());
                        AvailableKeys.Add(key);
                    }
                }
            }
        }

        #region IEditorInterface implementation

        public void SetNotification(BlinkStickClient.DataModel.CustomNotification notification)
        {
            this.Notification = notification as NotificationKeyboard;

            comboboxKey.Active = AvailableKeys.IndexOf(Notification.Key);
            checkbuttonAlt.Active = Notification.ModifierKeys.HasFlag(ModifierKeys.Alt);
            checkbuttonCtrl.Active = Notification.ModifierKeys.HasFlag(ModifierKeys.Control);
            checkbuttonWin.Active = Notification.ModifierKeys.HasFlag(ModifierKeys.Win);
        }

        public bool IsValid(Gtk.Window window)
        {
            return true;
        }

        public void UpdateNotification()
        {
            Notification.Key = AvailableKeys[comboboxKey.Active];

            Notification.ModifierKeys = 0;

            if (checkbuttonAlt.Active)
            {
                Notification.ModifierKeys |= ModifierKeys.Alt;
            }

            if (checkbuttonCtrl.Active)
            {
                Notification.ModifierKeys |= ModifierKeys.Control;
            }

            if (checkbuttonWin.Active)
            {
                Notification.ModifierKeys |= ModifierKeys.Win;
            }
        }

        #endregion
    }
}

