#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using Gtk;
using BlinkStickClient.Classes;
using System.Collections.Generic;

namespace BlinkStickClient
{
	public partial class BlinkStickManageForm : Gtk.Dialog
	{
		LedController _SelectedController;
		LedController SelectedController {
			get {
				return _SelectedController;
			}
			set {
				if (_SelectedController != value)
				{
					_SelectedController = value;
					UpdateFormComponents();
				}
			}
		}
		NotificationManager Manager;

		Gtk.ListStore DeviceListStore = new ListStore(typeof(LedController));

		public BlinkStickManageForm ()
		{
			this.Build ();
			this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
		}

		public static void ShowForm(NotificationManager manager)
		{
			BlinkStickManageForm form = new BlinkStickManageForm ();
			form.Manager = manager;
			form.PopulateForm();
			form.Run ();
			form.Destroy();
		}

		public void PopulateForm ()
		{
			Gtk.TreeViewColumn serialColumn = new Gtk.TreeViewColumn ();
			serialColumn.Title = "Serial";
			Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn ();
			nameColumn.Title = "Name";

			Gtk.CellRendererText serialCell = new Gtk.CellRendererText ();
			Gtk.CellRendererText nameCell = new Gtk.CellRendererText ();

			serialColumn.PackStart (serialCell, false);
			serialColumn.SetCellDataFunc (serialCell, new Gtk.TreeCellDataFunc (RenderSerial));

			nameColumn.PackEnd (nameCell, true);
			nameColumn.SetCellDataFunc (nameCell, new Gtk.TreeCellDataFunc (RenderName));

			treeviewDevices.Model = DeviceListStore;

			treeviewDevices.AppendColumn (serialColumn);
			treeviewDevices.AppendColumn (nameColumn);

			List<String> deviceNames = new List<string> ();
			foreach (LedController controller in Manager.Controllers) {
				deviceNames.Add (controller.DeviceVisibleName);
			}
			deviceNames.Sort ();

			foreach (String name in deviceNames) {
				DeviceListStore.AppendValues(Manager.FindControllerBySerialNumber(name));
			}

			UpdateFormComponents();
		}

		private void RenderSerial (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			if (model.GetValue (iter, 0) is LedController) {
				LedController controller = (LedController)model.GetValue (iter, 0);
				(cell as Gtk.CellRendererText).Text = controller.Device.Serial;
			}
		}

		private void RenderName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			if (model.GetValue (iter, 0) is LedController) {
				LedController controller = (LedController)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = controller.Device.InfoBlock1;
			}
		}

		protected void OnTreeviewDevicesCursorChanged (object sender, EventArgs e)
		{
			TreeModel model;
			TreeIter iter;

			TreeSelection selection = (sender as TreeView).Selection;

			if(selection.GetSelected(out model, out iter)){
				SelectedController = (LedController)model.GetValue (iter, 0);
			}
		}

		void UpdateFormComponents ()
		{
			if (SelectedController == null) {
				labelConnectedValue.Text = "";
				labelSerialNumberValue.Text = "";
				entryName.Text = "";
				entryData.Text = "";
				entryName.Sensitive = false;
				entryData.Sensitive = false;
				buttonApply.Sensitive = false;
				buttonReset.Sensitive = false;
				comboboxControl.Sensitive = false;
				comboboxControl.Active = -1;
			} else {
				labelConnectedValue.Text = "Yes";
				labelSerialNumberValue.Text = SelectedController.Device.Serial;
                entryName.Text = SelectedController.Device.InfoBlock1;
                entryData.Text = SelectedController.Device.InfoBlock2;
				comboboxControl.Active = (int)SelectedController.DataEntity.Control;
				entryName.Sensitive = true;
				entryData.Sensitive = true;
				buttonApply.Sensitive = true;
				buttonReset.Sensitive = true;
				comboboxControl.Sensitive = true;
			}
		}

		protected void OnButtonResetClicked (object sender, EventArgs e)
		{
			UpdateFormComponents();
		}

		protected void OnButtonApplyClicked (object sender, EventArgs e)
		{
            SelectedController.Device.InfoBlock1 = entryName.Text;
            SelectedController.Device.InfoBlock2 = entryData.Text;
			SelectedController.DataEntity.Control = (DeviceControlEnum)comboboxControl.Active;

			TreeModel model;
			TreeIter iter;

			if(treeviewDevices.Selection.GetSelected(out model, out iter)){
				DeviceListStore.EmitRowChanged(model.GetPath(iter), iter);
			}
		}
	}
}

