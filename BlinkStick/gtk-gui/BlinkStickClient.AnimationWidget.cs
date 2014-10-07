
// This file has been generated by the GUI designer. Do not modify.
namespace BlinkStickClient
{
	public partial class AnimationWidget
	{
		private global::Gtk.VBox vboxMain;
		
		private global::Gtk.HBox hboxColor;
		
		private global::Gtk.Label labelNumber;
		
		private global::Gtk.ComboBox comboboxMode;
		
		private global::Gtk.ColorButton buttonColor;
		
		private global::Gtk.Alignment alignment1;
		
		private global::Gtk.Button buttonUp;
		
		private global::Gtk.Button buttonDown;
		
		private global::Gtk.Button buttonDelete;
		
		private global::Gtk.HBox hboxSetColor;
		
		private global::Gtk.Label labelDelaySetColor;
		
		private global::Gtk.SpinButton spinbuttonSetColorDelay;
		
		private global::Gtk.HBox hboxBlink;
		
		private global::Gtk.Label labelDelayBlink;
		
		private global::Gtk.SpinButton spinbuttonBlinkDelay;
		
		private global::Gtk.Label labelRepeat;
		
		private global::Gtk.SpinButton spinbuttonBlinkRepeat;
		
		private global::Gtk.HBox hboxPulse;
		
		private global::Gtk.Label labelDurationPulse;
		
		private global::Gtk.SpinButton spinbuttonPulseDuration;
		
		private global::Gtk.Label labelTimesPulse;
		
		private global::Gtk.SpinButton spinbuttonPulseRepeat;
		
		private global::Gtk.HBox hboxMorph;
		
		private global::Gtk.Label labelDurationPulse1;
		
		private global::Gtk.SpinButton spinbuttonMorphDuration;
		
		private global::Gtk.HSeparator hseparator1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget BlinkStickClient.AnimationWidget
			global::Stetic.BinContainer.Attach (this);
			this.Events = ((global::Gdk.EventMask)(798964));
			this.ExtensionEvents = ((global::Gdk.ExtensionMode)(1));
			this.Name = "BlinkStickClient.AnimationWidget";
			// Container child BlinkStickClient.AnimationWidget.Gtk.Container+ContainerChild
			this.vboxMain = new global::Gtk.VBox ();
			this.vboxMain.Name = "vboxMain";
			this.vboxMain.Spacing = 6;
			this.vboxMain.BorderWidth = ((uint)(6));
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hboxColor = new global::Gtk.HBox ();
			this.hboxColor.Name = "hboxColor";
			// Container child hboxColor.Gtk.Box+BoxChild
			this.labelNumber = new global::Gtk.Label ();
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.LabelProp = global::Mono.Unix.Catalog.GetString ("#1");
			this.hboxColor.Add (this.labelNumber);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.labelNumber]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			w1.Padding = ((uint)(6));
			// Container child hboxColor.Gtk.Box+BoxChild
			this.comboboxMode = global::Gtk.ComboBox.NewText ();
			this.comboboxMode.AppendText (global::Mono.Unix.Catalog.GetString ("Set Color"));
			this.comboboxMode.AppendText (global::Mono.Unix.Catalog.GetString ("Blink"));
			this.comboboxMode.AppendText (global::Mono.Unix.Catalog.GetString ("Morph"));
			this.comboboxMode.AppendText (global::Mono.Unix.Catalog.GetString ("Pulse"));
			this.comboboxMode.Name = "comboboxMode";
			this.hboxColor.Add (this.comboboxMode);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.comboboxMode]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(6));
			// Container child hboxColor.Gtk.Box+BoxChild
			this.buttonColor = new global::Gtk.ColorButton ();
			this.buttonColor.CanFocus = true;
			this.buttonColor.Events = ((global::Gdk.EventMask)(784));
			this.buttonColor.Name = "buttonColor";
			this.hboxColor.Add (this.buttonColor);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.buttonColor]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hboxColor.Gtk.Box+BoxChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.hboxColor.Add (this.alignment1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.alignment1]));
			w4.Position = 3;
			// Container child hboxColor.Gtk.Box+BoxChild
			this.buttonUp = new global::Gtk.Button ();
			this.buttonUp.CanFocus = true;
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.UseUnderline = true;
			global::Gtk.Image w5 = new global::Gtk.Image ();
			w5.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-up", global::Gtk.IconSize.Menu);
			this.buttonUp.Image = w5;
			this.hboxColor.Add (this.buttonUp);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.buttonUp]));
			w6.Position = 4;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hboxColor.Gtk.Box+BoxChild
			this.buttonDown = new global::Gtk.Button ();
			this.buttonDown.CanFocus = true;
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.UseUnderline = true;
			global::Gtk.Image w7 = new global::Gtk.Image ();
			w7.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-down", global::Gtk.IconSize.Menu);
			this.buttonDown.Image = w7;
			this.hboxColor.Add (this.buttonDown);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.buttonDown]));
			w8.Position = 5;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hboxColor.Gtk.Box+BoxChild
			this.buttonDelete = new global::Gtk.Button ();
			this.buttonDelete.CanFocus = true;
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.UseUnderline = true;
			global::Gtk.Image w9 = new global::Gtk.Image ();
			w9.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-delete", global::Gtk.IconSize.Menu);
			this.buttonDelete.Image = w9;
			this.hboxColor.Add (this.buttonDelete);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hboxColor [this.buttonDelete]));
			w10.PackType = ((global::Gtk.PackType)(1));
			w10.Position = 6;
			w10.Expand = false;
			w10.Fill = false;
			this.vboxMain.Add (this.hboxColor);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxColor]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hboxSetColor = new global::Gtk.HBox ();
			this.hboxSetColor.Name = "hboxSetColor";
			this.hboxSetColor.Spacing = 6;
			// Container child hboxSetColor.Gtk.Box+BoxChild
			this.labelDelaySetColor = new global::Gtk.Label ();
			this.labelDelaySetColor.WidthRequest = 50;
			this.labelDelaySetColor.Name = "labelDelaySetColor";
			this.labelDelaySetColor.Xalign = 1F;
			this.labelDelaySetColor.LabelProp = global::Mono.Unix.Catalog.GetString ("Delay:");
			this.hboxSetColor.Add (this.labelDelaySetColor);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hboxSetColor [this.labelDelaySetColor]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hboxSetColor.Gtk.Box+BoxChild
			this.spinbuttonSetColorDelay = new global::Gtk.SpinButton (0D, 10000D, 1D);
			this.spinbuttonSetColorDelay.CanFocus = true;
			this.spinbuttonSetColorDelay.Name = "spinbuttonSetColorDelay";
			this.spinbuttonSetColorDelay.Adjustment.PageIncrement = 10D;
			this.spinbuttonSetColorDelay.ClimbRate = 1D;
			this.spinbuttonSetColorDelay.Numeric = true;
			this.hboxSetColor.Add (this.spinbuttonSetColorDelay);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hboxSetColor [this.spinbuttonSetColorDelay]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			this.vboxMain.Add (this.hboxSetColor);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxSetColor]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hboxBlink = new global::Gtk.HBox ();
			this.hboxBlink.Name = "hboxBlink";
			this.hboxBlink.Spacing = 6;
			// Container child hboxBlink.Gtk.Box+BoxChild
			this.labelDelayBlink = new global::Gtk.Label ();
			this.labelDelayBlink.WidthRequest = 50;
			this.labelDelayBlink.Name = "labelDelayBlink";
			this.labelDelayBlink.Xalign = 1F;
			this.labelDelayBlink.LabelProp = global::Mono.Unix.Catalog.GetString ("Delay:");
			this.hboxBlink.Add (this.labelDelayBlink);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hboxBlink [this.labelDelayBlink]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hboxBlink.Gtk.Box+BoxChild
			this.spinbuttonBlinkDelay = new global::Gtk.SpinButton (10D, 10000D, 1D);
			this.spinbuttonBlinkDelay.CanFocus = true;
			this.spinbuttonBlinkDelay.Name = "spinbuttonBlinkDelay";
			this.spinbuttonBlinkDelay.Adjustment.PageIncrement = 10D;
			this.spinbuttonBlinkDelay.ClimbRate = 1D;
			this.spinbuttonBlinkDelay.Numeric = true;
			this.spinbuttonBlinkDelay.Value = 10D;
			this.hboxBlink.Add (this.spinbuttonBlinkDelay);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hboxBlink [this.spinbuttonBlinkDelay]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			// Container child hboxBlink.Gtk.Box+BoxChild
			this.labelRepeat = new global::Gtk.Label ();
			this.labelRepeat.WidthRequest = 50;
			this.labelRepeat.Name = "labelRepeat";
			this.labelRepeat.Xalign = 1F;
			this.labelRepeat.LabelProp = global::Mono.Unix.Catalog.GetString ("Repeat:");
			this.hboxBlink.Add (this.labelRepeat);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hboxBlink [this.labelRepeat]));
			w17.Position = 2;
			w17.Expand = false;
			w17.Fill = false;
			// Container child hboxBlink.Gtk.Box+BoxChild
			this.spinbuttonBlinkRepeat = new global::Gtk.SpinButton (1D, 100D, 1D);
			this.spinbuttonBlinkRepeat.CanFocus = true;
			this.spinbuttonBlinkRepeat.Name = "spinbuttonBlinkRepeat";
			this.spinbuttonBlinkRepeat.Adjustment.PageIncrement = 10D;
			this.spinbuttonBlinkRepeat.ClimbRate = 1D;
			this.spinbuttonBlinkRepeat.Numeric = true;
			this.spinbuttonBlinkRepeat.Value = 1D;
			this.hboxBlink.Add (this.spinbuttonBlinkRepeat);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hboxBlink [this.spinbuttonBlinkRepeat]));
			w18.Position = 3;
			w18.Expand = false;
			w18.Fill = false;
			this.vboxMain.Add (this.hboxBlink);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxBlink]));
			w19.Position = 2;
			w19.Expand = false;
			w19.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hboxPulse = new global::Gtk.HBox ();
			this.hboxPulse.Name = "hboxPulse";
			this.hboxPulse.Spacing = 6;
			// Container child hboxPulse.Gtk.Box+BoxChild
			this.labelDurationPulse = new global::Gtk.Label ();
			this.labelDurationPulse.WidthRequest = 50;
			this.labelDurationPulse.Name = "labelDurationPulse";
			this.labelDurationPulse.Xalign = 1F;
			this.labelDurationPulse.LabelProp = global::Mono.Unix.Catalog.GetString ("Duration:");
			this.hboxPulse.Add (this.labelDurationPulse);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hboxPulse [this.labelDurationPulse]));
			w20.Position = 0;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hboxPulse.Gtk.Box+BoxChild
			this.spinbuttonPulseDuration = new global::Gtk.SpinButton (10D, 10000D, 1D);
			this.spinbuttonPulseDuration.CanFocus = true;
			this.spinbuttonPulseDuration.Name = "spinbuttonPulseDuration";
			this.spinbuttonPulseDuration.Adjustment.PageIncrement = 10D;
			this.spinbuttonPulseDuration.ClimbRate = 1D;
			this.spinbuttonPulseDuration.Numeric = true;
			this.spinbuttonPulseDuration.Value = 10D;
			this.hboxPulse.Add (this.spinbuttonPulseDuration);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hboxPulse [this.spinbuttonPulseDuration]));
			w21.Position = 1;
			w21.Expand = false;
			w21.Fill = false;
			// Container child hboxPulse.Gtk.Box+BoxChild
			this.labelTimesPulse = new global::Gtk.Label ();
			this.labelTimesPulse.WidthRequest = 50;
			this.labelTimesPulse.Name = "labelTimesPulse";
			this.labelTimesPulse.Xalign = 1F;
			this.labelTimesPulse.LabelProp = global::Mono.Unix.Catalog.GetString ("Repeat:");
			this.hboxPulse.Add (this.labelTimesPulse);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hboxPulse [this.labelTimesPulse]));
			w22.Position = 2;
			w22.Expand = false;
			w22.Fill = false;
			// Container child hboxPulse.Gtk.Box+BoxChild
			this.spinbuttonPulseRepeat = new global::Gtk.SpinButton (1D, 100D, 1D);
			this.spinbuttonPulseRepeat.CanFocus = true;
			this.spinbuttonPulseRepeat.Name = "spinbuttonPulseRepeat";
			this.spinbuttonPulseRepeat.Adjustment.PageIncrement = 10D;
			this.spinbuttonPulseRepeat.ClimbRate = 1D;
			this.spinbuttonPulseRepeat.Numeric = true;
			this.spinbuttonPulseRepeat.Value = 1D;
			this.hboxPulse.Add (this.spinbuttonPulseRepeat);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hboxPulse [this.spinbuttonPulseRepeat]));
			w23.Position = 3;
			w23.Expand = false;
			w23.Fill = false;
			this.vboxMain.Add (this.hboxPulse);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxPulse]));
			w24.Position = 3;
			w24.Expand = false;
			w24.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hboxMorph = new global::Gtk.HBox ();
			this.hboxMorph.Name = "hboxMorph";
			this.hboxMorph.Spacing = 6;
			// Container child hboxMorph.Gtk.Box+BoxChild
			this.labelDurationPulse1 = new global::Gtk.Label ();
			this.labelDurationPulse1.WidthRequest = 50;
			this.labelDurationPulse1.Name = "labelDurationPulse1";
			this.labelDurationPulse1.Xalign = 1F;
			this.labelDurationPulse1.LabelProp = global::Mono.Unix.Catalog.GetString ("Duration:");
			this.hboxMorph.Add (this.labelDurationPulse1);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.hboxMorph [this.labelDurationPulse1]));
			w25.Position = 0;
			w25.Expand = false;
			w25.Fill = false;
			// Container child hboxMorph.Gtk.Box+BoxChild
			this.spinbuttonMorphDuration = new global::Gtk.SpinButton (10D, 10000D, 1D);
			this.spinbuttonMorphDuration.CanFocus = true;
			this.spinbuttonMorphDuration.Name = "spinbuttonMorphDuration";
			this.spinbuttonMorphDuration.Adjustment.PageIncrement = 10D;
			this.spinbuttonMorphDuration.ClimbRate = 1D;
			this.spinbuttonMorphDuration.Numeric = true;
			this.spinbuttonMorphDuration.Value = 10D;
			this.hboxMorph.Add (this.spinbuttonMorphDuration);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hboxMorph [this.spinbuttonMorphDuration]));
			w26.Position = 1;
			w26.Expand = false;
			w26.Fill = false;
			this.vboxMain.Add (this.hboxMorph);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxMorph]));
			w27.Position = 4;
			w27.Expand = false;
			w27.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.vboxMain.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hseparator1]));
			w28.Position = 5;
			w28.Expand = false;
			w28.Fill = false;
			this.Add (this.vboxMain);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.comboboxMode.Changed += new global::System.EventHandler (this.OnComboboxModeChanged);
			this.buttonDelete.Clicked += new global::System.EventHandler (this.OnButtonDeleteClicked);
		}
	}
}
