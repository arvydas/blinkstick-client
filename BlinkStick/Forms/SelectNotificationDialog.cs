using System;
using System.Collections.Generic;
using BlinkStickClient.DataModel;
using Gtk;

namespace BlinkStickClient
{
    public partial class SelectNotificationDialog : Gtk.Dialog
    {
        Dictionary<String, TreeIter> Categories = new Dictionary<string, TreeIter>();

        private NotificationRegistry.NotificationRegistryEntry _SelectedType;
        public NotificationRegistry.NotificationRegistryEntry SelectedType 
        {
            get
            {
                return _SelectedType;
            }
            set
            {
                if (_SelectedType != value)
                {
                    _SelectedType = value;
                    UpdateUI();
                }
            }
        }

        public SelectNotificationDialog()
        {
            this.Build();

            TreeViewColumn nameColumn = new TreeViewColumn ();
            nameColumn.Title = "Notification";
            CellRendererText nameCell = new CellRendererText ();
            nameColumn.PackStart (nameCell, true);
            nameColumn.SetCellDataFunc(nameCell, NameRenderer);

            treeviewNotifications.AppendColumn (nameColumn);

            nameColumn.AddAttribute (nameCell, "text", 0);

            TreeStore typeListStore = new TreeStore (typeof (String), typeof (NotificationRegistry.NotificationRegistryEntry));

            foreach (NotificationRegistry.NotificationRegistryEntry entry in NotificationRegistry.NotificationTypes)
            {
                TreeIter iter;

                if (Categories.ContainsKey(entry.Category))
                {
                    iter = Categories[entry.Category];
                }
                else
                {
                    iter = typeListStore.AppendValues (entry.Category, null);
                    Categories[entry.Category] = iter;
                }

                typeListStore.AppendValues (iter, null, entry);
            }

            treeviewNotifications.Model = typeListStore;

            UpdateUI();
        }

        private void NameRenderer(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {    
            object value = model.GetValue(iter, 1);

            if (value is NotificationRegistry.NotificationRegistryEntry)
            {
                NotificationRegistry.NotificationRegistryEntry myclass = value as NotificationRegistry.NotificationRegistryEntry;
                if (myclass != null)
                {
                    (cell as CellRendererText).Text = myclass.Name;
                }
            }
            else
            {
                (cell as CellRendererText).Text = (String)model.GetValue(iter, 0);
            }
        }

        protected void OnTreeviewNotificationsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            TreeSelection selection = (sender as TreeView).Selection;

            if(selection.GetSelected(out model, out iter)){
                SelectedType = (NotificationRegistry.NotificationRegistryEntry)model.GetValue (iter, 1);
            }

        }

        private void UpdateUI()
        {
            if (SelectedType != null)
            {
                labelCategoryInfo.Text = SelectedType.Category;
                labelNameInfo.Text = SelectedType.Name;
                labelDescriptionInfo.Text = SelectedType.Description;
            }
            else
            {
                labelCategoryInfo.Text = "";
                labelNameInfo.Text = "";
                labelDescriptionInfo.Text = "";
            }
        }
    }
}