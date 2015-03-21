using System;
using BlinkStickClient.DataModel;
using log4net;
using Gtk;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NotificationsWidget : Gtk.Bin
    {
        public ApplicationDataModel DataModel;

        protected static readonly ILog log = LogManager.GetLogger("Main");  

        Gtk.ListStore NotificationListStore = new ListStore(typeof(Notification), typeof(String), typeof(String), typeof(String));

        private Notification _SelectedNotification = null;

        public Notification SelectedNotification {
            get {
                return _SelectedNotification;
            }
            set {
                if (_SelectedNotification != value)
                {
                    _SelectedNotification = value;
                }
            }
        }

        public Gtk.Window ParentForm;

        public NotificationsWidget()
        {
            this.Build();

            log.Debug( "Setting up treeview");

            Gtk.TreeViewColumn enabledColumn = new Gtk.TreeViewColumn ();
            enabledColumn.Title = "";

            Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn ();
            nameColumn.Title = "Name";

            Gtk.TreeViewColumn typeColumn = new Gtk.TreeViewColumn ();
            typeColumn.Title = "Type";

            Gtk.TreeViewColumn blinkStickColumn = new Gtk.TreeViewColumn ();
            blinkStickColumn.Title = "BlinkStick";

            Gtk.TreeViewColumn patternColumn = new Gtk.TreeViewColumn ();
            patternColumn.Title = "Pattern";

            Gtk.CellRendererPixbuf enabledCell = new Gtk.CellRendererPixbuf ();
            Gtk.CellRendererText nameCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText typeCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText blinkStickCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText patternCell = new Gtk.CellRendererText ();

            enabledColumn.PackEnd (enabledCell, false);
            blinkStickColumn.PackEnd (blinkStickCell, false);
            nameColumn.PackEnd (nameCell, true);
            typeColumn.PackEnd (typeCell, false);
            patternColumn.PackEnd (patternCell, false);

            enabledColumn.SetCellDataFunc (enabledCell, new Gtk.TreeCellDataFunc (RenderEnabledCell));
            nameColumn.SetCellDataFunc (nameCell, new Gtk.TreeCellDataFunc (RenderNameCell));
            typeColumn.SetCellDataFunc (typeCell, new Gtk.TreeCellDataFunc (RenderTypeCell));
            blinkStickColumn.SetCellDataFunc (blinkStickCell, new Gtk.TreeCellDataFunc (RenderBlinkStickCell));
            patternColumn.SetCellDataFunc (patternCell, new Gtk.TreeCellDataFunc (RenderPatternCell));

            treeviewEvents.AppendColumn (typeColumn);
            treeviewEvents.AppendColumn (patternColumn);
            treeviewEvents.AppendColumn (blinkStickColumn);
            treeviewEvents.AppendColumn (nameColumn);

            treeviewEvents.AppendColumn (enabledColumn);
            treeviewEvents.AppendColumn ("", new Gtk.CellRendererPixbuf(), "stock_id", 1);
            treeviewEvents.AppendColumn ("", new Gtk.CellRendererPixbuf(), "stock_id", 2);
            treeviewEvents.AppendColumn ("", new Gtk.CellRendererPixbuf(), "stock_id", 3);

            treeviewEvents.Columns[3].Expand = true;

            treeviewEvents.Model = NotificationListStore;
        }

        public void Initialize()
        {
            log.Debug("Adding notifications to the tree");
            foreach (Notification e in DataModel.Notifications) {
                NotificationListStore.AppendValues (e, "gtk-edit", "gtk-copy", "gtk-delete");
            } 
        }

        public void NotificationUpdated(Notification notification)
        {
            TreeIter iter;
            Boolean searchMore = NotificationListStore.GetIterFirst(out iter);
            while (searchMore)
            {
                if (NotificationListStore.GetValue(iter, 0) == notification)
                {
                    NotificationListStore.EmitRowChanged(NotificationListStore.GetPath(iter), iter);
                }

                searchMore = NotificationListStore.IterNext(ref iter);
            }
        }

        protected void OnTreeviewEventsRowActivated (object o, Gtk.RowActivatedArgs args)
        {
            EditNotification();
        }

        protected void OnTreeviewEventsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            TreeSelection selection = (sender as TreeView).Selection;

            if(selection.GetSelected(out model, out iter)){
                SelectedNotification = (Notification)model.GetValue (iter, 0);

                TreePath path;
                TreeViewColumn column;
                (sender as TreeView).GetCursor(out path, out column);

                if (column == (sender as TreeView).Columns[7]) //Delete clicked
                {
                    DataModel.Notifications.Remove(SelectedNotification);
                    NotificationListStore.Remove(ref iter);
                    DataModel.Save();
                }
                else if (column == (sender as TreeView).Columns[6]) //Copy clicked
                {
                    Notification notification = SelectedNotification.Copy();
                    notification.Name = "";
                    if (EditNotification(notification, "Copy Notification"))
                    {
                        NotificationListStore.AppendValues(notification, "gtk-edit", "gtk-copy", "gtk-delete");
                        DataModel.Notifications.Add(notification);
                    }
                }
                else if (column == (sender as TreeView).Columns[5]) //Edit clicked
                {
                    EditNotification();
                }
            }
        }
        private void EditNotification()
        {
            if (SelectedNotification != null && EditNotification(SelectedNotification))
            {
                log.DebugFormat("Notification {0} edit complete", SelectedNotification.ToString());
                DataModel.Save();
                DataModel.Notifications.NotifyUpdate(SelectedNotification);
            }
        }

        private Boolean EditNotification(Notification notification, String title = "Edit Notification")
        {
            int response;

            using (EditNotificationDialog editDialog = 
                new EditNotificationDialog(title, this.ParentForm, this.DataModel, notification))
            {
                response = editDialog.Run();
                editDialog.Destroy();
            }

            log.DebugFormat("Edit notification dialog response {0}", (ResponseType)response);

            return response == (int)ResponseType.Ok;
        }

        private void RenderEnabledCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is Notification) {
                Notification notification = (Notification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererPixbuf).StockId = notification.Enabled ? "gtk-yes" : "gtk-no";
            }
        }

        private void RenderNameCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is Notification) {
                Notification notification = (Notification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = notification.Name;
            }
        }

        private void RenderTypeCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is Notification) {
                Notification notification = (Notification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = notification.GetTypeName();
            }
        }

        private void RenderBlinkStickCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is Notification) {
                Notification notification = (Notification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = notification.BlinkStickSerial;
            }
        }

        private void RenderPatternCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue(iter, 0) is PatternNotification)
            {
                PatternNotification notification = (PatternNotification)model.GetValue(iter, 0);
                (cell as Gtk.CellRendererText).Text = notification.Pattern == null ? "" : notification.Pattern.Name;
            }
            else
            {
                (cell as Gtk.CellRendererText).Text = "";
            }
        }

        protected void OnButtonAddNotificationClicked(object sender, EventArgs e)
        {
            int response;

            Type notificationType = typeof(Notification);

            using (SelectNotificationDialog dialog = new SelectNotificationDialog())
            {
                response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    notificationType = dialog.SelectedType.NotificationType;
                }
                dialog.Destroy();
            }

            if (response == (int)ResponseType.Ok)
            {

                Notification notification = (Notification)Activator.CreateInstance(notificationType);

                if (EditNotification(notification, "New Notification"))
                {
                    NotificationListStore.AppendValues(notification, "gtk-edit", "gtk-copy", "gtk-delete");
                    DataModel.Notifications.Add(notification);
                    DataModel.Save();
                }
            }
        }
    }
}

