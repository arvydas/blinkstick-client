using System;
using Gtk;
using BlinkStickClient.Classes;

namespace BlinkStickClient
{
    public partial class PatternDialog : Gtk.Dialog
    {
        Gtk.ListStore PatternListStore = new ListStore(typeof(Pattern));

        public Pattern _SelectedPattern = null;

        public Pattern SelectedPattern {
            get {
                return _SelectedPattern;
            }
            set {
                if (_SelectedPattern != value)
                {
                    _SelectedPattern = value;
                    UpdateButtons();
                }
            }
        }


        public PatternDialog()
        {
            this.Build();

            Gtk.TreeViewColumn patternNameColumn = new Gtk.TreeViewColumn ();
            patternNameColumn.Title = "Name";

            // Create the text cell that will display the artist name
            Gtk.CellRendererText patternNameCell = new Gtk.CellRendererText ();

            patternNameColumn.PackStart (patternNameCell, true);

            patternNameColumn.SetCellDataFunc (patternNameCell, new Gtk.TreeCellDataFunc (RenderPatternName));

            treeviewPatterns.Model = PatternListStore;

            treeviewPatterns.AppendColumn (patternNameColumn);

            PatternListStore.AppendValues (new Pattern("Pattern1"));
            PatternListStore.AppendValues (new Pattern("Pattern2"));
            PatternListStore.AppendValues (new Pattern("Pattern3"));
            PatternListStore.AppendValues (new Pattern("Pattern4"));
            PatternListStore.AppendValues (new Pattern("Pattern5"));

            UpdateButtons();

            for (int i = 0; i < 10; i++)
            {
                AnimationWidget anim = new AnimationWidget();
                anim.Index = i;
                vbox2.PackStart(anim);

            }

            Button btn = new Button();
            btn.Label = "Add new";
            vbox2.PackStart(btn);
            btn.Show();
        }

        void UpdateButtons()
        {
            deleteAction.Sensitive = SelectedPattern != null;
            propertiesAction.Sensitive = SelectedPattern != null;
        }

        public static void ShowForm()
        {
            PatternDialog form = new PatternDialog ();
            form.Run ();
            form.Destroy();
        }


        private void RenderPatternName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, 0) is Pattern) {
                Pattern pattern = (Pattern)model.GetValue (iter, 0);
                (cell as Gtk.CellRendererText).Text = pattern.Name;
            }
        }

        protected void OnNewActionActivated (object sender, EventArgs e)
        {
            Pattern pattern = new Pattern();

            if (EditPatternDialog.ShowForm(pattern))
            {
                PatternListStore.AppendValues(pattern);
            }
        }

        protected void OnDeleteActionActivated (object sender, EventArgs e)
        {
            TreeModel modelx;
            TreeIter iter;

            if(treeviewPatterns.Selection.GetSelected(out modelx, out iter)){
                PatternListStore.Remove(ref iter);
            }
        }


        protected void OnPropertiesActionActivated (object sender, EventArgs e)
        {
            EditPatternDialog.ShowForm(SelectedPattern);
        }

        protected void OnTreeviewPatternsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            if((sender as TreeView).Selection.GetSelected(out model, out iter)){
                SelectedPattern = (Pattern)model.GetValue (iter, 0);
            }
        }
    }
}

