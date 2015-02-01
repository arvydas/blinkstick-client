using System;
using Gtk;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class OverviewWidget : Gtk.Bin
    {
        BlinkStickDevices BlinkStickDeviceList = new BlinkStickDevices();

        private BlinkStickDeviceSettings _SelectedBlinkStick;

        private BlinkStickDeviceSettings SelectedBlinkStick
        {
            get
            {
                return _SelectedBlinkStick;
            }

            set 
            {
                if (_SelectedBlinkStick != value)
                {
                    _SelectedBlinkStick = value;

                }
            }
        }

        public OverviewWidget()
        {
            this.Build();

            Gdk.Pixbuf image = Gdk.Pixbuf.LoadFromResource("BlinkStickClient.blinkstick.png");
            image = image.ScaleSimple(image.Width / 2, image.Height / 2, Gdk.InterpType.Nearest);
            imageBlinkStickPreview.Pixbuf = image;

            hbox1.PackStart(new ColorPaletteWidget());
    
            RefreshDevices();

            CellRendererText blinkStickDeviceSettingsCell = new CellRendererText();
            comboboxDevices.PackStart(blinkStickDeviceSettingsCell, true);
            comboboxDevices.AddAttribute (blinkStickDeviceSettingsCell, "text", 0);
            comboboxDevices.SetCellDataFunc(blinkStickDeviceSettingsCell, BlinkStickDeviceSettingsClassRenderer);

            ListStore store = new ListStore(typeof (BlinkStickDeviceSettings));

            foreach (BlinkStickDeviceSettings entity in BlinkStickDeviceList.Devices) {
                store.AppendValues(entity);
            }

            comboboxDevices.Model = store;

            UpdateUI();
        }

        private void BlinkStickDeviceSettingsClassRenderer(CellLayout cell_layout, CellRenderer cell, TreeModel model, TreeIter iter)
        {    
            BlinkStickDeviceSettings myclass = model.GetValue(iter, 0) as BlinkStickDeviceSettings;
            if (myclass != null)
            {
                (cell as CellRendererText).Text = myclass.ToString();
                //(cell as CellRendererText).Alignment = Pango.Alignment.Left;
                cell.Xalign = 0;
            }
        }

        protected void OnButtonConfigureBlinkStickClicked (object sender, EventArgs e)
        {
            ConfigureBlinkStickDialog dialog = new ConfigureBlinkStickDialog();
            dialog.Run();
            dialog.Destroy();
        }

        public void RefreshDevices()
        {
            foreach (BlinkStickDotNet.BlinkStick led in BlinkStickDotNet.BlinkStick.FindAll())
            {
                BlinkStickDeviceList.AddIfDoesNotExist(led);
            }
        }

        private void UpdateUI()
        {
            buttonConfigure.Sensitive = comboboxDevices.Active != -1;
            buttonDelete.Sensitive = comboboxDevices.Active != -1;
            blinkstickinfowidget2.UpdateUI(SelectedBlinkStick);
        }

        protected void OnComboboxDevicesChanged (object sender, EventArgs e)
        {
            TreeIter iter;

            (sender as Gtk.ComboBox).GetActiveIter(out iter);
            SelectedBlinkStick = (BlinkStickDeviceSettings)((sender as Gtk.ComboBox).Model.GetValue(iter, 0));

            UpdateUI();
        }
    }
}

