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
                    LoadAnimations();
                }
            }
        }

        private DataModel _Data;
        public DataModel Data {
            set {

                if (_Data != value)
                {
                    _Data = value;
                    PatternListStore.Clear();
                    foreach (Pattern pattern in Data.Patterns)
                    {
                        PatternListStore.AppendValues(pattern);
                    }
                }
            }

            get {
                return _Data;
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

            PatternListStore.SetSortFunc(0, delegate(TreeModel model, TreeIter a, TreeIter b) {
                Pattern p1 = (Pattern)model.GetValue(a, 0);
                Pattern p2 = (Pattern)model.GetValue(b, 0);
                return String.Compare(p1.Name, p2.Name);
            });
            PatternListStore.SetSortColumnId(0, SortType.Ascending);

            treeviewPatterns.Model = PatternListStore;

            treeviewPatterns.AppendColumn (patternNameColumn);


            UpdateButtons();
        }

        void UpdateButtons()
        {
            deleteAction.Sensitive = SelectedPattern != null;
            propertiesAction.Sensitive = SelectedPattern != null;
        }

        void LoadAnimations()
        {
            foreach (Widget child in vbox2.AllChildren)
            {
                child.Destroy();
            }


            if (SelectedPattern != null)
            {
                int i = 1;

                foreach (Animation animation in SelectedPattern.Animations)
                {
                    AnimationWidget widget = CreateAnimationWidget(animation, i);
                    vbox2.PackStart(widget, false, false, 0);

                    i++;
                }

                Button btn = new Button();
                btn.Label = "Add new";
                btn.Clicked += (sender, e) => {
                    Animation animation = new Animation();
                    SelectedPattern.Animations.Add(animation);

                    AnimationWidget widget = CreateAnimationWidget(animation, SelectedPattern.Animations.Count);
                    vbox2.PackStart(widget, false, false, 0);
                    vbox2.ReorderChild(widget, SelectedPattern.Animations.Count - 1);
                    ReorderAnimations();
                };
                btn.Show();

                vbox2.PackStart(btn, false, false, 10);
                ReorderAnimations();
            }
            vbox2.Show();
        }

        private AnimationWidget CreateAnimationWidget(Animation animation, int index)
        {
            AnimationWidget widget = new AnimationWidget();
            widget.Index = index;
            widget.AnimationObject = animation;

            widget.DeleteAnimation += (sender, e) => {
                AnimationWidget w = (AnimationWidget)sender;
                SelectedPattern.Animations.Remove(w.AnimationObject);
                w.Destroy();
                ReorderAnimations();
            };

            widget.MoveUp += (sender, e) => {
                MoveAnimationBy((AnimationWidget)sender, -1);
            };

            widget.MoveDown += (sender, e) => {
                MoveAnimationBy((AnimationWidget)sender, 1);
            };

            return widget;
        }

        private void MoveAnimationBy(AnimationWidget widget, int count)
        {
            int oldIndex = SelectedPattern.Animations.IndexOf(widget.AnimationObject);

            SelectedPattern.Animations.RemoveAt(oldIndex);
            SelectedPattern.Animations.Insert(oldIndex + count, widget.AnimationObject);

            vbox2.ReorderChild(widget, oldIndex + count);
            ReorderAnimations();
        }

        private void ReorderAnimations()
        {
            int i = 1;
            foreach (Widget widget in vbox2.Children)
            {
                if (widget is AnimationWidget)
                {
                    ((AnimationWidget)widget).Index = i;
                    ((AnimationWidget)widget).Count = SelectedPattern.Animations.Count;
                    i++;
                }
            }
        }

        public static void ShowForm(DataModel data)
        {
            PatternDialog form = new PatternDialog ();
            form.Data = data;
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
                pattern.Animations.Add(new Animation());
                Data.Patterns.Add(pattern);
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
                SelectedPattern = (Pattern)model.GetValue(iter, 0);
            }
        }
    }
}

