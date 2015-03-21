using System;
using BlinkStickClient.DataModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Gtk;

namespace BlinkStickClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class EventsWidget : Gtk.Bin
    {

        private ApplicationDataModel _DataModel;
        public ApplicationDataModel DataModel
        {
            set
            {
                _DataModel = value;
                HookIntoDataModel(value);
            }
            get
            {
                return _DataModel;
            }
        }

        ListStore EventListStore = new ListStore(typeof(Notification));

        uint RefreshTimer;

        public EventsWidget()
        {
            this.Build();

            Gtk.TreeViewColumn timeColumn = new Gtk.TreeViewColumn ();
            timeColumn.Title = "Time";
            timeColumn.Resizable = true;

            Gtk.TreeViewColumn notificationColumn = new Gtk.TreeViewColumn ();
            notificationColumn.Title = "Notification";
            notificationColumn.Resizable = true;

            Gtk.TreeViewColumn messageColumn = new Gtk.TreeViewColumn ();
            messageColumn.Title = "Message";

            Gtk.CellRendererText timeCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText notificationCell = new Gtk.CellRendererText ();
            Gtk.CellRendererText messageCell = new Gtk.CellRendererText ();

            timeColumn.PackEnd (timeCell, false);
            notificationColumn.PackEnd (notificationCell, false);
            messageColumn.PackEnd (messageCell, false);

            timeColumn.SetCellDataFunc (timeCell, new Gtk.TreeCellDataFunc (RenderTimeCell));
            notificationColumn.SetCellDataFunc (notificationCell, new Gtk.TreeCellDataFunc (RenderNotificationCell));
            messageColumn.SetCellDataFunc (messageCell, new Gtk.TreeCellDataFunc (RenderMessageCell));

            treeviewEvents.AppendColumn (timeColumn);
            treeviewEvents.AppendColumn (notificationColumn);
            treeviewEvents.AppendColumn (messageColumn);

            EventListStore.SetSortFunc(0, delegate(TreeModel model, TreeIter a, TreeIter b) {
                TriggeredEvent s1 = (TriggeredEvent)model.GetValue(a, 0);
                TriggeredEvent s2 = (TriggeredEvent)model.GetValue(b, 0);
                return DateTime.Compare(s1.TimeStamp, s2.TimeStamp);
            });
            EventListStore.SetSortColumnId(0, SortType.Descending);

            treeviewEvents.Model = EventListStore;

            RefreshTimer = GLib.Timeout.Add(5000, new GLib.TimeoutHandler(UpdateRows));
        }

        private bool UpdateRows()
        {
            TreeIter iter;
            if (EventListStore.GetIterFirst(out iter))
            {
                do
                {
                    TriggeredEvent ev = (TriggeredEvent)EventListStore.GetValue(iter, 0);
                    if (ev.LastDisplay != ev.TimeStamp.TimeAgo())
                    {
                        TreePath path = EventListStore.GetPath(iter);

                        EventListStore.EmitRowChanged(path, iter);
                    }
                }
                while (EventListStore.IterNext(ref iter));
            }

            return true;
        }

        private void HookIntoDataModel(ApplicationDataModel dataModel)
        {
            foreach (TriggeredEvent ev in dataModel.TriggeredEvents)
            {
                EventListStore.AppendValues(ev);
            }

            dataModel.TriggeredEvents.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (TriggeredEvent ev in e.NewItems)
                    {
                        EventListStore.AppendValues(ev);
                    }
                }
            };
        }

        private void RenderTimeCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is TriggeredEvent) {
                TriggeredEvent ev = (TriggeredEvent)model.GetValue (iter, 0);
                ev.LastDisplay = ev.TimeStamp.TimeAgo();
                (cell as Gtk.CellRendererText).Text = ev.LastDisplay;
            }
        }

        private void RenderNotificationCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is TriggeredEvent) {
                TriggeredEvent ev = (TriggeredEvent)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = ev.Notification.Name;
            }
        }

        private void RenderMessageCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is TriggeredEvent) {
                TriggeredEvent ev = (TriggeredEvent)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = ev.Message;
            }
        }

        protected void OnButtonClearClicked(object sender, EventArgs e)
        {
            EventListStore.Clear();
            DataModel.TriggeredEvents.Clear();
            DataModel.Save();
        }
    }
}

