using System;
using System.Collections.Generic;
using BlinkStickClient.DataModel;
using Gtk;

namespace BlinkStickClient
{
    public partial class SelectNotificationDialog : Gtk.Dialog
    {
        Dictionary<String, TreeIter> Categories = new Dictionary<string, TreeIter>();

        TreeModelFilter filter;
        TreeStore typeListStore;

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

        public ApplicationDataModel DataModel;

        public SelectNotificationDialog()
        {
            this.Build();

            TreeViewColumn nameColumn = new TreeViewColumn ();
            nameColumn.Title = "Notification";

            CellRendererPixbuf iconCell = new CellRendererPixbuf();
            nameColumn.PackStart(iconCell, false);
            nameColumn.AddAttribute(iconCell, "pixbuf", 2);

            CellRendererText nameCell = new CellRendererText ();
            nameColumn.PackStart (nameCell, true);
            nameColumn.SetCellDataFunc(nameCell, NameRenderer);

            treeviewNotifications.AppendColumn (nameColumn);


            nameColumn.AddAttribute (nameCell, "text", 0);

            typeListStore = new TreeStore (typeof(String), typeof(NotificationRegistry.NotificationRegistryEntry), typeof (Gdk.Pixbuf));

            foreach (NotificationRegistry.NotificationRegistryEntry entry in NotificationRegistry.NotificationTypes)
            {
                TreeIter iter;

                if (Categories.ContainsKey(entry.Category))
                {
                    iter = Categories[entry.Category];
                }
                else
                {
                    Gdk.Pixbuf icon = null;

                    try
                    {
                        icon = Gdk.Pixbuf.LoadFromResource ("BlinkStickClient.Resources.notifications.group-" + entry.Category.ToLower() + ".png");
                    }
                    catch
                    {
                    }

                    iter = typeListStore.AppendValues (entry.Category, null, icon);
                    Categories[entry.Category] = iter;
                }

                typeListStore.AppendValues (iter, null, entry, entry.Icon);
            }

            // Create the filter and tell it to use the musicListStore as it's base Model
            filter = new TreeModelFilter (typeListStore, null);

            // Specify the function that determines which rows to filter out and which ones to display
            filter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTree);

            treeviewNotifications.Model = filter;

            UpdateUI();

            #if !DEBUG
            vbox2.Remove(checkbuttonDisplayOnlySupported);
            #endif
        }

        private bool FilterTree (Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (!checkbuttonDisplayOnlySupported.Active)
            {
                return true;
            }

            object value = model.GetValue(iter, 1);

            if (value is NotificationRegistry.NotificationRegistryEntry)
            {
                NotificationRegistry.NotificationRegistryEntry myclass = value as NotificationRegistry.NotificationRegistryEntry;
                return myclass.IsSupported;
            }
            else
            {
                TreeIter citer;
                if (typeListStore.IterHasChild(iter))
                {
                    typeListStore.IterChildren(out citer, iter);

                    do
                    {
                        NotificationRegistry.NotificationRegistryEntry childType = (NotificationRegistry.NotificationRegistryEntry)model.GetValue(citer, 1);
                        if (childType != null && childType.IsSupported)
                        {
                            return true;
                        }
                    }
                    while (typeListStore.IterNext(ref citer));

                }

                return false;
            }
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
            Boolean canAdd = false;

            if (SelectedType != null)
            {
                labelCategoryInfo.Text = SelectedType.Category;
                labelNameInfo.Text = SelectedType.Name;
                labelDescriptionInfo.Text = SelectedType.Description;

                canAdd = DataModel.CanAddNotification(SelectedType);

                labelUniqueWarning.Visible = !canAdd;
            }
            else
            {
                labelCategoryInfo.Text = "";
                labelNameInfo.Text = "";
                labelDescriptionInfo.Text = "";
                labelUniqueWarning.Visible = false;
            }

            this.buttonOk.Sensitive = SelectedType != null && SelectedType.IsSupported && canAdd;
        }

        protected void OnCheckbuttonDisplayOnlySupportedToggled (object sender, EventArgs e)
        {
            filter.Refilter();
        }
    }
}