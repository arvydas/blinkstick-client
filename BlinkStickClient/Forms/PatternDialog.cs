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
        public PatternDialog()
        {
            this.Build();
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

