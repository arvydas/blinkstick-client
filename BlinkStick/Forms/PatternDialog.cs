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

            Pattern pattern = new Pattern();
            pattern.Name = "Pattern 1";

            Animation animation = new Animation();
            animation.AnimationType = AnimationTypeEnum.SetColor;
            pattern.Animations.Add(animation);

            animation = new Animation();
            animation.AnimationType = AnimationTypeEnum.Morph;
            pattern.Animations.Add(animation);

            animation = new Animation();
            animation.AnimationType = AnimationTypeEnum.Pulse;
            pattern.Animations.Add(animation);

            /*
            animation = new Animation();
            animation.AnimationType = AnimationTypeEnum.Blink;
            pattern.Animations.Add(animation);
            */

            PatternListStore.AppendValues (pattern);
            /*
            PatternListStore.AppendValues (new Pattern("Pattern1"));
            PatternListStore.AppendValues (new Pattern("Pattern2"));
            PatternListStore.AppendValues (new Pattern("Pattern3"));
            PatternListStore.AppendValues (new Pattern("Pattern4"));
            PatternListStore.AppendValues (new Pattern("Pattern5"));
            */

            UpdateButtons();

            /*
            for (int i = 0; i < 10; i++)
            {
                AnimationWidget anim = new AnimationWidget();
                anim.Index = i;
                vbox2.PackStart(anim);

            }
            */

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
                    AnimationWidget widget = new AnimationWidget();
                    widget.Index = i;
                    widget.AnimationObject = animation;

                    widget.DeleteAnimation += (sender, e) => {
                        AnimationWidget w = (AnimationWidget)sender;
                        SelectedPattern.Animations.Remove(w.AnimationObject);
                        w.Destroy();
                        ReorderAnimations();
                    };

                    vbox2.PackStart(widget, false, false, 0);

                    i++;
                }

                Button btn = new Button();
                btn.Label = "Add new";
                vbox2.PackStart(btn, false, false, 10);
                btn.Show();
            }
        }

        private void ReorderAnimations()
        {
            int i = 1;
            foreach (Widget widget in vbox2.Children)
            {
                if (widget is AnimationWidget)
                {
                    ((AnimationWidget)widget).Index = i;
                    i++;
                }
            }
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
                LoadAnimations();
            }
        }
    }
}

