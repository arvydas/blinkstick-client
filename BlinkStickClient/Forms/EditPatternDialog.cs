using System;
using BlinkStickClient.Classes;
using BlinkStickClient.DataModel;
using Gtk;

namespace BlinkStickClient
{
    public partial class EditPatternDialog : Gtk.Dialog
    {
        private ApplicationDataModel _Data;
        private Pattern _Pattern;

        public EditPatternDialog()
        {
            this.Build();
        }

        public static Boolean ShowForm(Pattern pattern, ApplicationDataModel data, string title)
        {
            EditPatternDialog form = new EditPatternDialog ();
            form.Title = title;
            form._Data = data;
            form._Pattern = pattern;

            //Object to controls
            form.entryName.Text = pattern.Name;
            form.spinbuttonStart.Value = pattern.LedFirstIndex;
            form.spinbuttonEnd.Value = pattern.LedLastIndex;

            form.Response += (object o, ResponseArgs args) =>
            {
                if (args.ResponseId == ResponseType.Ok)
                {
                    //Controls to object
                    pattern.Name = form.entryName.Text;
                    pattern.LedFirstIndex = form.spinbuttonStart.ValueAsInt;
                    pattern.LedLastIndex = form.spinbuttonEnd.ValueAsInt;
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

            if (spinbuttonStart.ValueAsInt > spinbuttonEnd.ValueAsInt)
            {
                Utils.MessageBox.Show(this, "First LED index can not be greater than last LED index", MessageType.Error);
                return;
            }

            this.Respond(ResponseType.Ok);
        }
    }
}

