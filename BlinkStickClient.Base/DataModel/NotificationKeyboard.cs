using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BlinkStickClient.Utils;

namespace BlinkStickClient.DataModel
{
    public class NotificationKeyboard  : PatternNotification
    {
        public ModifierKeys ModifierKeys;

        [JsonConverter(typeof(StringEnumConverter))]
        public Keys Key;

        private KeyboardHook KeyboardHook;

        public override string GetTypeName()
        {
            return "Keyboard";
        }

        public NotificationKeyboard()
        {
        }

        public override CustomNotification Copy(CustomNotification notification)
        {
            if (notification == null)
            {
                notification = new NotificationKeyboard();
            }

            ((NotificationKeyboard)notification).ModifierKeys = this.ModifierKeys;
            ((NotificationKeyboard)notification).Key = this.Key;

            return base.Copy(notification);
        }

        public override void Start()
        {
            base.Start();

            if (KeyboardHook == null)
            {
                KeyboardHook = new KeyboardHook();
                KeyboardHook.KeyPressed += (sender, e) => {
                    OnTriggered(String.Format("Triggered with {0}", this.KeyToString()));
                };
            }

            try
            {
                KeyboardHook.RegisterHotKey(this.ModifierKeys, this.Key);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Unable to register hotkey {0}", e.Message);
            }
        }

        public override void Stop()
        {
            if (!Running)
                return;

            if (KeyboardHook != null)
            {
                KeyboardHook.Dispose();
                KeyboardHook = null;
            }

            base.Stop();
        }

        public override bool IsSupported()
        {
            return HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows;
        }

        public override bool RequiresMonitoring()
        {
            return true;
        }

        private String KeyToString()
        {
            String result = "";

            if (this.ModifierKeys.HasFlag(ModifierKeys.Control))
            {
                if (result != "")
                {
                    result += "+";
                }
                result += "Ctrl";    
            }

            if (this.ModifierKeys.HasFlag(ModifierKeys.Alt))
            {
                if (result != "")
                {
                    result += "+";
                }
                result += "Alt";    
            }

            if (this.ModifierKeys.HasFlag(ModifierKeys.Win))
            {
                if (result != "")
                {
                    result += "+";
                }
                result += "Win";    
            }

            if (result != "")
            {
                result += " ";
            }

            result += this.Key.ToString();

            return result;
        }
    }
}

