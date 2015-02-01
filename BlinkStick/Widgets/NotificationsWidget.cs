using System;
using BlinkStickClient.Classes;
using log4net;
using Gtk;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class NotificationsWidget : Gtk.Bin
    {
        protected static readonly ILog log = LogManager.GetLogger("Main");  

        Boolean IgnoreActivation = false;

        Gtk.ListStore EventListStore = new ListStore(typeof(CustomNotification));

        public NotificationManager Manager;

        private CustomNotification _SelectedNotification = null;

        public CustomNotification SelectedNotification {
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

            Gtk.TreeViewColumn eventTitleColumn = new Gtk.TreeViewColumn ();
            eventTitleColumn.Title = "Name";

            Gtk.TreeViewColumn eventTypeColumn = new Gtk.TreeViewColumn ();
            eventTypeColumn.Title = "Type";

            Gtk.TreeViewColumn eventColorColumn = new Gtk.TreeViewColumn ();
            eventColorColumn.Title = "Color";

            // Create the text cell that will display the artist name
            Gtk.CellRendererText eventTitleCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText eventTypeCell = new Gtk.CellRendererText ();
            Gtk.CellRendererPixbuf eventColorCell = new Gtk.CellRendererPixbuf ();

            // Add the cell to the column
            eventColorColumn.PackStart (eventColorCell, false);
            eventTypeColumn.PackEnd (eventTypeCell, true);
            eventTitleColumn.PackEnd (eventTitleCell, true);

            // Tell the Cell Renderers which items in the model to display
            //eventTitleColumn.AddAttribute (eventTitleCell, "name", 0);
            eventTitleColumn.SetCellDataFunc (eventTitleCell, new Gtk.TreeCellDataFunc (RenderEventName));
            eventTypeColumn.SetCellDataFunc (eventTypeCell, new Gtk.TreeCellDataFunc (RenderEventType));
            eventColorColumn.SetCellDataFunc (eventColorCell, new Gtk.TreeCellDataFunc (RenderEventColor));

            treeviewEvents.Model = EventListStore;

            treeviewEvents.AppendColumn (eventColorColumn);
            treeviewEvents.AppendColumn (eventTypeColumn);
            treeviewEvents.AppendColumn (eventTitleColumn);
        }

        public void Initialize()
        {
            log.Debug("Adding notifications to the tree");
            //Gtk.TreeIter customEventRoot = EventListStore.AppendValues(new TriggeredEvent("Custom"));
            foreach (CustomNotification e in Manager.Notifications) {
                //EventListStore.AppendValues(customEventRoot, e);
                EventListStore.AppendValues (e);
            }        

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            editAction.Sensitive = SelectedNotification != null;
            deleteAction.Sensitive = SelectedNotification != null;
            //checkAction.Sensitive = SelectedNotification != null;
            copyAction.Sensitive = SelectedNotification != null;
            activeAction.Sensitive = SelectedNotification != null;
            IgnoreActivation = true;
            activeAction.Active = SelectedNotification != null && SelectedNotification.Active;
            IgnoreActivation = false;
        }

        public void NotificationUpdated(CustomNotification notification)
        {
            TreeIter iter;
            Boolean searchMore = EventListStore.GetIterFirst(out iter);
            while (searchMore)
            {
                if (EventListStore.GetValue(iter, 0) == notification)
                {
                    EventListStore.EmitRowChanged(EventListStore.GetPath(iter), iter);
                }

                searchMore = EventListStore.IterNext(ref iter);
            }
        }

        protected void OnTreeviewEventsRowActivated (object o, Gtk.RowActivatedArgs args)
        {
            EditNotificationForm.ShowForm(SelectedNotification, Manager);
        }

        protected void OnTreeviewEventsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            TreeSelection selection = (sender as TreeView).Selection;

            if(selection.GetSelected(out model, out iter)){
                SelectedNotification = (CustomNotification)model.GetValue (iter, 0);
            }
        }
        protected void OnNewActionActivated (object sender, EventArgs e)
        {
            CustomNotification newEvent = SelectNotificationTypeForm.ShowForm();

            if (newEvent != null && EditNotificationForm.ShowForm(newEvent, Manager))
            {
                EventListStore.AppendValues(newEvent);
                Manager.AddNotification (newEvent);
                newEvent.InitializeServices();
                Manager.Save();
            }
        }
        protected void OnCopyActionActivated (object sender, EventArgs e)
        {
            CustomNotification ev = SelectedNotification.Copy();
            if (EditNotificationForm.ShowForm(ev, Manager))
            {
                EventListStore.AppendValues(ev);
                Manager.AddNotification (ev);
                ev.InitializeServices();
            }
        }

        protected void OnEditActionActivated (object sender, EventArgs e)
        {
            EditNotificationForm.ShowForm(SelectedNotification, Manager);
            Manager.Save();
        }
        protected void OnDeleteActionActivated (object sender, EventArgs e)
        {
            TreeModel modelx;
            TreeIter iter;

            TreeSelection selection = treeviewEvents.Selection;

            if(selection.GetSelected(out modelx, out iter)){
                SelectedNotification.FinalizeServices();
                Manager.RemoveNotification(SelectedNotification);
                EventListStore.Remove(ref iter);
                Manager.Save();
            }
        }
        protected void OnActiveActionToggled (object sender, EventArgs e)
        {
            if (IgnoreActivation)
                return;

            SelectedNotification.Active = activeAction.Active;
        }

        private void RenderEventName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is CustomNotification) {
                CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = tevent.Name;
            }
        }

        private void RenderEventType (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is CustomNotification) {
                CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = tevent.GetTypeName();
            }
        }

        private void RenderEventColor (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is CustomNotification) {
                CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
                Gdk.Pixbuf pb = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, 16, 16);
                pb.Fill ((uint)(0xff + tevent.Color.R * 0x1000000 + tevent.Color.G * 0x10000 + tevent.Color.B * 0x100));
                (cell as Gtk.CellRendererPixbuf).Pixbuf = pb;
            }
        }



    }
}

