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

        Gtk.ListStore NotificationListStore = new ListStore(typeof(Notification));

        private Notification _SelectedNotification = null;

        public Notification SelectedNotification {
            get {
                return _SelectedNotification;
            }
            set {
                if (_SelectedNotification != value)
                {
                    _SelectedNotification = value;
                    UpdateButtons();
                }
            }
        }

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
            enabledColumn.AddAttribute(enabledCell, "stock-id", 0);
            blinkStickColumn.PackEnd (blinkStickCell, false);
            nameColumn.PackEnd (nameCell, false);
            typeColumn.PackEnd (typeCell, false);
            patternColumn.PackEnd (patternCell, false);

            enabledColumn.SetCellDataFunc (enabledCell, new Gtk.TreeCellDataFunc (RenderEnabledCell));
            nameColumn.SetCellDataFunc (nameCell, new Gtk.TreeCellDataFunc (RenderNameCell));
            typeColumn.SetCellDataFunc (typeCell, new Gtk.TreeCellDataFunc (RenderTypeCell));
            blinkStickColumn.SetCellDataFunc (blinkStickCell, new Gtk.TreeCellDataFunc (RenderBlinkStickCell));
            patternColumn.SetCellDataFunc (patternCell, new Gtk.TreeCellDataFunc (RenderPatternCell));

            treeviewEvents.AppendColumn (enabledColumn);
            treeviewEvents.AppendColumn (typeColumn);
            treeviewEvents.AppendColumn (patternColumn);
            treeviewEvents.AppendColumn (blinkStickColumn);
            treeviewEvents.AppendColumn (nameColumn);



            treeviewEvents.Model = NotificationListStore;
        }

        public void Initialize()
        {
            log.Debug("Adding notifications to the tree");
            foreach (Notification e in DataModel.Notifications) {
                NotificationListStore.AppendValues (e);
            } 

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            editAction.Sensitive = SelectedNotification != null;
            deleteAction.Sensitive = SelectedNotification != null;
            copyAction.Sensitive = SelectedNotification != null;
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
            EditNotification(SelectedNotification);
        }

        protected void OnTreeviewEventsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            TreeSelection selection = (sender as TreeView).Selection;

            if(selection.GetSelected(out model, out iter)){
                SelectedNotification = (Notification)model.GetValue (iter, 0);
            }
        }
        protected void OnNewActionActivated (object sender, EventArgs e)
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
                    NotificationListStore.AppendValues(notification);
                    DataModel.Notifications.Add(notification);
                    DataModel.Save();
                }
            }
        }
        protected void OnCopyActionActivated (object sender, EventArgs e)
        {
            Notification notification = SelectedNotification.Copy();
            if (EditNotification(notification, "Copy Notification"))
            {
                NotificationListStore.AppendValues(notification);
                DataModel.Notifications.Add(notification);
            }
        }

        private Boolean EditNotification(Notification notification, String title = "Edit Notification")
        {
            int response;

            using (EditNotificationDialog editDialog = new EditNotificationDialog())
            {
                editDialog.Title = title;
                editDialog.DataModel = this.DataModel;
                editDialog.RefreshDevices();
                editDialog.Notification = notification;
                response = editDialog.Run();
                editDialog.Destroy();
            }

            return response == (int)ResponseType.Ok;
        }

        protected void OnEditActionActivated (object sender, EventArgs e)
        {
            EditNotification(SelectedNotification);
            DataModel.Save();
        }
        protected void OnDeleteActionActivated (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            TreeSelection selection = treeviewEvents.Selection;

            if(selection.GetSelected(out model, out iter)){
                DataModel.Notifications.Remove(SelectedNotification);
                NotificationListStore.Remove(ref iter);
                DataModel.Save();
            }
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
                (cell as Gtk.CellRendererText).Text = notification.Pattern.Name;
            }
            else
            {
                (cell as Gtk.CellRendererText).Text = "";
            }
        }
    }
}

