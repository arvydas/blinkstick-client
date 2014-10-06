using System;
using BlinkStickClient.Classes;
using Gtk;

namespace BlinkStickClient
{
    public partial class EditPatternDialog : Gtk.Dialog
    {
        public EditPatternDialog()
        {
            this.Build();
        }

        public static Boolean ShowForm(Pattern pattern)
        {
            Boolean result = false;

            EditPatternDialog form = new EditPatternDialog ();
            form.entryName.Text = pattern.Name;

            form.Response += (object o, ResponseArgs args) => {
                if (args.ResponseId == ResponseType.Ok)
                {
                    pattern.Name = form.entryName.Text;
                    result = true;
                }
            };

            form.Run ();
            form.Destroy();

            return result;
        }
    }
}

