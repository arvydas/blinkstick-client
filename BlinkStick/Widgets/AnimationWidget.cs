using System;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class AnimationWidget : Gtk.Bin
    {
        public int Index {
            set {
                labelNumber.Text = "#" + value.ToString(); 
            }
        }

        public AnimationWidget()
        {
            this.Build();

            comboboxMode.Active = 0;

            OnComboboxModeChanged(this, null);
        }

        protected void OnComboboxModeChanged (object sender, EventArgs e)
        {
            hboxSetColor.Visible = comboboxMode.Active == 0;
            hboxSetColor.Sensitive = comboboxMode.Active == 0;
            hboxSetColor.NoShowAll = !(comboboxMode.Active == 0);

            hboxBlink.Visible = comboboxMode.Active == 1;
            hboxBlink.Sensitive = comboboxMode.Active == 1;
            hboxBlink.NoShowAll = !(comboboxMode.Active == 1);

            hboxPulse.Visible = comboboxMode.Active >= 2;
            hboxPulse.Sensitive = comboboxMode.Active >= 2;
            hboxPulse.NoShowAll = !(comboboxMode.Active >= 2);

            this.Show();
        }
    }
}

