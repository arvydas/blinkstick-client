using System;
using BlinkStickClient.Classes;
using BlinkStickClient.DataModel;
using Gtk;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class AnimationWidget : Gtk.Bin
    {
        private int _Index;
        public int Index {
            get {
                return _Index;
            }
            set {
                _Index = value;
                labelNumber.Text = "#" + value.ToString(); 
                ButtonVisibility(ShowEditControls);
            }
        }

        private int _Count;
        public int Count {
            get {
                return _Count;
            }
            set {
                _Count = value;
                ButtonVisibility(ShowEditControls);
            }
        }

        private Boolean Loading = false;

        public event EventHandler DeleteAnimation;

        protected void OnDeleteAnimation()
        {
            if (DeleteAnimation != null)
                DeleteAnimation(this, new EventArgs());
        }

        public event EventHandler MoveUp;

        protected void OnMoveUp()
        {
            if (MoveUp != null)
                MoveUp(this, new EventArgs());
        }

        public event EventHandler MoveDown;

        protected void OnMoveDown()
        {
            if (MoveDown != null)
                MoveDown(this, new EventArgs());
        }


        private Animation _AnimationObject;
        public Animation AnimationObject {
            get { 
                return _AnimationObject;
            }

            set {
                if (_AnimationObject != value)
                {
                    _AnimationObject = value;
                    LoadAnimationObject();
                }
            }
        }

        public Boolean ShowEditControls { get; set; }

        public AnimationWidget()
        {
            this.Build();

            comboboxMode.Active = 0;

            OnComboboxModeChanged(this, null);

            /*
            this.EnterNotifyEvent += (o, args) => {
                ButtonVisibility(true);
            };

            this.LeaveNotifyEvent += (o, args) =>  {
                ButtonVisibility(false);
            };
            */

            ShowEditControls = false;

            UpdateUIControls();

            /*
            foreach (Widget widget in this.AllChildren)
            {
                widget.EnterNotifyEvent += delegate(object o, EnterNotifyEventArgs args)
                {
                    args.RetVal = false;
                };
                widget.LeaveNotifyEvent += delegate(object o, LeaveNotifyEventArgs args)
                {
                    args.RetVal = false;
                };
            }
            */
        }

        private void ButtonVisibility(Boolean visible) 
        {
            buttonUp.Visible = visible;
            buttonUp.Sensitive = visible && Index != 1;
            buttonUp.NoShowAll = !visible;

            buttonDown.Visible = visible;
            buttonDown.Sensitive = visible && Index != Count;
            buttonDown.NoShowAll = !visible;

            buttonDelete.Visible = visible;
            buttonDelete.Sensitive = visible && Count != 1;
            buttonDelete.NoShowAll = !visible;
        }

        protected void OnComboboxModeChanged (object sender, EventArgs e)
        {
            UpdateUIControls();

            if (AnimationObject != null)
            {
                switch (comboboxMode.Active)
                {
                    case 0:
                        AnimationObject.AnimationType = AnimationTypeEnum.SetColor;
                        break;
                    case 1:
                        AnimationObject.AnimationType = AnimationTypeEnum.Blink;
                        break;
                    case 2:
                        AnimationObject.AnimationType = AnimationTypeEnum.Morph;
                        break;
                    case 3:
                        AnimationObject.AnimationType = AnimationTypeEnum.Pulse;
                        break;
                }
            }

            this.Show();
        }

        private void UpdateUIControls()
        {
            hboxSetColor.Visible = comboboxMode.Active == 0 && ShowEditControls;
            hboxSetColor.Sensitive = comboboxMode.Active == 0 && ShowEditControls;
            hboxSetColor.NoShowAll = !(comboboxMode.Active == 0) && !ShowEditControls;

            hboxBlink.Visible = comboboxMode.Active == 1 && ShowEditControls;
            hboxBlink.Sensitive = comboboxMode.Active == 1 && ShowEditControls;
            hboxBlink.NoShowAll = !(comboboxMode.Active == 1) && !ShowEditControls;

            hboxMorph.Visible = comboboxMode.Active == 2 && ShowEditControls;
            hboxMorph.Sensitive = comboboxMode.Active == 2 && ShowEditControls;
            hboxMorph.NoShowAll = !(comboboxMode.Active == 2) && !ShowEditControls;

            hboxPulse.Visible = comboboxMode.Active == 3 && ShowEditControls;
            hboxPulse.Sensitive = comboboxMode.Active == 3 && ShowEditControls;
            hboxPulse.NoShowAll = !(comboboxMode.Active == 3) && !ShowEditControls;

            ButtonVisibility(ShowEditControls);
        }

        private void LoadAnimationObject()
        {
            Loading = true;

            switch (AnimationObject.AnimationType)
            {
                case AnimationTypeEnum.SetColor:
                    comboboxMode.Active = 0;
                    break;
                case AnimationTypeEnum.Blink:
                    comboboxMode.Active = 1;
                    break;
                case AnimationTypeEnum.Morph:
                    comboboxMode.Active = 2;
                    break;
                case AnimationTypeEnum.Pulse:
                    comboboxMode.Active = 3;
                    break;
            }


            spinbuttonSetColorDelay.Value = AnimationObject.DelaySetColor;

            spinbuttonBlinkDelay.Value = AnimationObject.DurationBlink;
            spinbuttonBlinkRepeat.Value = AnimationObject.RepeatBlink;

            spinbuttonPulseDuration.Value = AnimationObject.DurationPulse;
            spinbuttonPulseRepeat.Value = AnimationObject.RepeatPulse;

            spinbuttonMorphDuration.Value = AnimationObject.DurationMorph;

            buttonColor.Color = AnimationObject.GtkColor;

            OnComboboxModeChanged(this, null);

            Loading = false;
        }

        protected void OnButtonDeleteClicked (object sender, EventArgs e)
        {
            OnDeleteAnimation();
        }

        protected void OnButtonUpClicked (object sender, EventArgs e)
        {
            OnMoveUp();
        }

        protected void OnButtonDownClicked (object sender, EventArgs e)
        {
            OnMoveDown();
        }

        protected void OnButtonColorColorSet (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.GtkColor = buttonColor.Color;
        }

        protected void OnSpinbuttonSetColorDelayValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.DelaySetColor = (sender as SpinButton).ValueAsInt;
        }

        protected void OnSpinbuttonBlinkDelayValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.DurationBlink = (sender as SpinButton).ValueAsInt;
        }

        protected void OnSpinbuttonBlinkRepeatValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.RepeatBlink = (sender as SpinButton).ValueAsInt;
        }

        protected void OnSpinbuttonPulseDurationValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.DurationPulse = (sender as SpinButton).ValueAsInt;
        }

        protected void OnSpinbuttonPulseRepeatValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.RepeatPulse = (sender as SpinButton).ValueAsInt;
        }

        protected void OnSpinbuttonMorphDurationValueChanged (object sender, EventArgs e)
        {
            if (Loading)
                return;

            AnimationObject.DurationMorph = (sender as SpinButton).ValueAsInt;
        }

        protected void OnButtonPropertiesClicked (object sender, EventArgs e)
        {
            ShowEditControls = !ShowEditControls;
            UpdateUIControls();
        }
    }
}

