using System;
using BlinkStickClient.Classes;
using Gtk;

namespace BlinkStickClient
{
    public partial class EditPatternDialog : Gtk.Dialog
    {
        private DataModel _Data;
        private Pattern _Pattern;

        public EditPatternDialog()
        {
            this.Build();
        }

        public static Boolean ShowForm(Pattern pattern, DataModel data)
        {
            EditPatternDialog form = new EditPatternDialog ();
            form._Data = data;
            form._Pattern = pattern;

            //Object to controls
            form.entryName.Text = pattern.Name;

            form.Response += (object o, ResponseArgs args) =>
            {
                if (args.ResponseId == ResponseType.Ok)
                {
                    //Controls to object
                    pattern.Name = form.entryName.Text;
                }
            };

            Boolean result = form.Run () == (int)ResponseType.Ok;

            form.Destroy();

            return result;
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
            if (entryName.Text == "") {
                Utils.MessageBox.Show(this, "Name cannot be blank", MessageType.Error);
                return;
            }

            if (_Data.PatternNameExists(_Pattern, entryName.Text)) {
                Utils.MessageBox.Show(this, "Pattern name already exists", MessageType.Error);
                return;
            }

            this.Respond(ResponseType.Ok);
        }
    }
}

