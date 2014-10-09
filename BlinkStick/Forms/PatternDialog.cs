using System;
using Gtk;
using Gdk;
using BlinkStickClient.Classes;
using BlinkStickClient.Utils;
using System.ComponentModel;
using System.Threading;
using BlinkStickDotNet;

namespace BlinkStickClient
{
    public partial class PatternDialog : Gtk.Dialog
    {
        Gtk.ListStore PatternListStore = new ListStore(typeof(String), typeof(Pattern), typeof(String), typeof(String));

        const int PatternColumn = 1;

        BlinkStickDotNet.BlinkStick led;
        Boolean ignoreNextClick = false;

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

        private Pattern PlayingPattern = null;
        private BackgroundWorker PatternPlayer = null;

        private ApplicationDataModel _DataModel;
        public ApplicationDataModel DataModel {
            set {

                if (_DataModel != value)
                {
                    _DataModel = value;
                    PatternListStore.Clear();
                    foreach (Pattern pattern in DataModel.Patterns)
                    {
                        PatternListStore.AppendValues("gtk-media-play", pattern, "gtk-copy", "gtk-delete");
                    }
                }
            }

            get {
                return _DataModel;
            }
        }


        public PatternDialog()
        {
            this.Build();

            Gtk.TreeViewColumn patternNameColumn = new Gtk.TreeViewColumn ();
            patternNameColumn.Title = "Name";
            Gtk.CellRendererText patternNameCell = new Gtk.CellRendererText ();
            patternNameColumn.PackStart (patternNameCell, true);
            patternNameColumn.SetCellDataFunc (patternNameCell, new Gtk.TreeCellDataFunc (RenderPatternName));

            treeviewPatterns.AppendColumn ("Play", new Gtk.CellRendererPixbuf(), "stock_id", 0);
            treeviewPatterns.AppendColumn (patternNameColumn);
            treeviewPatterns.AppendColumn ("Copy", new Gtk.CellRendererPixbuf(), "stock_id", 2);
            treeviewPatterns.AppendColumn ("Delete", new Gtk.CellRendererPixbuf(), "stock_id", 3);

            treeviewPatterns.Columns[3].Expand = false;
            treeviewPatterns.Columns[2].Expand = false;
            treeviewPatterns.Columns[1].Expand = true;

            PatternListStore.SetSortFunc(0, delegate(TreeModel model, TreeIter a, TreeIter b) {
                Pattern p1 = (Pattern)model.GetValue(a, PatternColumn);
                Pattern p2 = (Pattern)model.GetValue(b, PatternColumn);
                if (p1 == null || p2 == null) 
                    return 0;
                return String.Compare(p1.Name, p2.Name);
            });

            PatternListStore.SetSortColumnId(0, SortType.Ascending);

            treeviewPatterns.Model = PatternListStore;

            //These events get lost in the designer
            treeviewPatterns.RowActivated += OnTreeviewPatternsRowActivated;
            treeviewPatterns.CursorChanged += OnTreeviewPatternsCursorChanged;

            UpdateButtons();
        }

        void UpdateButtons()
        {
            buttonProperties.Sensitive = SelectedPattern != null;
            buttonAddAnimation.Sensitive = SelectedPattern != null;
        }

        void LoadAnimations()
        {
            foreach (Widget child in vboxAnimations.AllChildren)
            {
                child.Destroy();
            }


            if (SelectedPattern != null)
            {
                int i = 1;

                foreach (Animation animation in SelectedPattern.Animations)
                {
                    AnimationWidget widget = CreateAnimationWidget(animation, i);
                    vboxAnimations.PackStart(widget, false, false, 0);

                    i++;
                }

                ReorderAnimations();
            }
            vboxAnimations.Show();
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

            vboxAnimations.ReorderChild(widget, oldIndex + count);
            ReorderAnimations();
        }

        private void ReorderAnimations()
        {
            int i = 1;
            foreach (Widget widget in vboxAnimations.Children)
            {
                if (widget is AnimationWidget)
                {
                    ((AnimationWidget)widget).Index = i;
                    ((AnimationWidget)widget).Count = SelectedPattern.Animations.Count;
                    i++;
                }
            }
        }

        public static void ShowForm(ApplicationDataModel data)
        {
            PatternDialog form = new PatternDialog ();
            form.DataModel = data;
            form.Run ();
            form.StopPattern();
            form.Destroy();
        }


        private void RenderPatternName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            if (model.GetValue (iter, PatternColumn) is Pattern) {
                Pattern pattern = (Pattern)model.GetValue (iter, PatternColumn);
                (cell as Gtk.CellRendererText).Text = pattern.Name;
            }
        }
        protected void OnTreeviewPatternsCursorChanged (object sender, EventArgs e)
        {
            TreeModel model;
            TreeIter iter;

            if((sender as TreeView).Selection.GetSelected(out model, out iter)){
                SelectedPattern = (Pattern)model.GetValue(iter, PatternColumn);

                if (ignoreNextClick)
                {
                    ignoreNextClick = false;
                    return;
                }

                TreePath path;
                TreeViewColumn column;
                (sender as TreeView).GetCursor(out path, out column);

                if (column == (sender as TreeView).Columns[3]) //Delete clicked
                {
                    PatternListStore.Remove(ref iter);
                    DataModel.Patterns.Remove(SelectedPattern);
                    SelectedPattern = null;
                }
                else if (column == (sender as TreeView).Columns[2]) //Copy clicked
                {
                    Pattern pattern = new Pattern();
                    pattern.Assign(SelectedPattern);
                    pattern.Name = "";

                    if (EditPatternDialog.ShowForm(pattern, DataModel, "Copy Pattern " + SelectedPattern.Name))
                    {
                        AddPattern(pattern);
                    }
                }
                else if (column == (sender as TreeView).Columns[0]) //Play clicked
                {
                    if (SelectedPattern == PlayingPattern)
                    {
                        StopPattern();
                    }
                    else
                    {
                        PlayPattern(SelectedPattern);
                        model.SetValue(iter, 0, "gtk-media-stop");
                    }
                }
            }
        }

        protected void OnTreeviewPatternsRowActivated (object o, RowActivatedArgs args)
        {
            EditPatternDialog.ShowForm(SelectedPattern, DataModel, "Edit Pattern");
        }

        protected void OnButtonPropertiesClicked (object sender, EventArgs e)
        {
            EditPatternDialog.ShowForm(SelectedPattern, DataModel, "Edit Pattern");
        }

        protected void OnButtonAddPatternClicked (object sender, EventArgs e)
        {
            Pattern pattern = new Pattern();

            if (EditPatternDialog.ShowForm(pattern, DataModel, "Add Pattern"))
            {
                AddPattern(pattern);
            }
        }

        protected void OnButtonAddAnimationClicked (object sender, EventArgs e)
        {
            Animation animation = new Animation();
            SelectedPattern.Animations.Add(animation);

            AnimationWidget widget = CreateAnimationWidget(animation, SelectedPattern.Animations.Count);
            vboxAnimations.PackStart(widget, false, false, 0);
            vboxAnimations.ReorderChild(widget, SelectedPattern.Animations.Count - 1);
            ReorderAnimations();
        }

        private void AddPattern(Pattern pattern)
        {
            PatternListStore.AppendValues("gtk-media-play", pattern, "gtk-copy", "gtk-delete");
            pattern.Animations.Add(new Animation());
            DataModel.Patterns.Add(pattern);

            TreeIter iterator;
            PatternListStore.GetIterFirst(out iterator);

            do
            {
                if (pattern == (Pattern)PatternListStore.GetValue(iterator, PatternColumn))
                {
                    ignoreNextClick = true;
                    treeviewPatterns.SetCursor(PatternListStore.GetPath(iterator), treeviewPatterns.Columns[PatternColumn], false);
                    break;
                }
            } 
            while (PatternListStore.IterNext(ref iterator));
        }

        private void StopPattern()
        {
            if (PatternPlayer != null)
            {
                led.Stop();
                PatternPlayer.CancelAsync();

                while (PatternPlayer != null && PatternPlayer.IsBusy)
                {
                    if (Gtk.Application.EventsPending())
                        Gtk.Application.RunIteration();

                    Thread.Sleep(1);
                }
            }
        }

        private void PlayPattern(Pattern pattern)
        {
            StopPattern();

            PlayingPattern = pattern;

            RgbColor color = blinkstickemulatorwidget.LedColor.ToRgbColor();

            led = new BlinkStickDotNet.BlinkStick();

            PatternPlayer = new BackgroundWorker();
            PatternPlayer.DoWork += (object sender, DoWorkEventArgs e) => {

                byte r = color.R;
                byte g = color.G;
                byte b = color.B;

                led.SendColor += (object o, BlinkStickDotNet.SendColorEventArgs ee) => {
                    Gtk.Application.Invoke (delegate {
                        blinkstickemulatorwidget.LedColor = new Color(ee.R, ee.G, ee.B);
                    });

                    r = ee.R;
                    g = ee.G;
                    b = ee.B;

                    ee.SendToDevice = false;
                };

                led.ReceiveColor += (object o, BlinkStickDotNet.ReceiveColorEventArgs ee) => {
                    ee.R = r;
                    ee.B = b;
                    ee.G = g;
                };

                foreach (Animation animation in pattern.Animations)
                {
                    if (((BackgroundWorker)sender).CancellationPending)
                        return;

                    switch (animation.AnimationType) {
                        case AnimationTypeEnum.SetColor:
                            led.SetColor(animation.Color);
                            led.WaitThread(animation.DelaySetColor);
                            break;
                        case AnimationTypeEnum.Blink:
                            led.Blink(animation.Color, animation.RepeatBlink, animation.DurationBlink);
                            break;
                        case AnimationTypeEnum.Pulse:
                            led.Pulse(animation.Color, animation.RepeatPulse, animation.DurationPulse);
                            break;
                        case AnimationTypeEnum.Morph:
                            led.Morph(animation.Color, animation.DurationMorph);
                            break;
                    }
                }
            };

            PatternPlayer.RunWorkerCompleted += (sender, e) => {
                SetPatternIconToStop(pattern);
                PlayingPattern = null;
                PatternPlayer = null;
                led = null;
            };

            PatternPlayer.WorkerSupportsCancellation = true;
            PatternPlayer.RunWorkerAsync();
        }

        private void SetPatternIconToStop(Pattern pattern)
        {
            TreeIter iterator;
            PatternListStore.GetIterFirst(out iterator);

            do
            {
                if (pattern == (Pattern)PatternListStore.GetValue(iterator, PatternColumn))
                {
                    PatternListStore.SetValue(iterator, 0, "gtk-media-play");
                    break;
                }
            } 
            while (PatternListStore.IterNext(ref iterator));
        }
    }
}

