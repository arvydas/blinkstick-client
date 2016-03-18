using System;
using Gtk;
using Gdk;
using BlinkStickClient.Utils;
using System.ComponentModel;
using System.Threading;
using BlinkStickDotNet;
using BlinkStickClient.DataModel;

namespace BlinkStickClient
{
    public partial class PatternDialog : Gtk.Dialog
    {
        private global::BlinkStickClient.PatternEditorWidget patternEditorWidget;

        public PatternDialog()
        {
            this.Build();

            // Container child vbox2.Gtk.Box+BoxChild
            this.patternEditorWidget = new global::BlinkStickClient.PatternEditorWidget ();
            this.patternEditorWidget.Events = ((global::Gdk.EventMask)(256));
            this.patternEditorWidget.Name = "patternEditorWidget";
            this.vbox2.Add (this.patternEditorWidget);
            global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.patternEditorWidget]));
            w2.Position = 0;

            this.patternEditorWidget.Show();
        }

        public static void ShowForm(ApplicationDataModel dataModel, Pattern selectPattern = null)
        {
            PatternDialog dialog = new PatternDialog ();
            dialog.patternEditorWidget.PreselectedPattern = selectPattern;
            dialog.patternEditorWidget.DataModel = dataModel;

            dialog.Run ();

            //Required to unbind events
            dialog.patternEditorWidget.DataModel = null;
            dialog.Destroy();

            dataModel.OnPatternsUpdated();
        }
    }
}

