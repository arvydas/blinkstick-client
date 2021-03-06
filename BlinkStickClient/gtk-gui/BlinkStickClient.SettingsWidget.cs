
// This file has been generated by the GUI designer. Do not modify.
namespace BlinkStickClient
{
	public partial class SettingsWidget
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment;
		
		private global::Gtk.CheckButton checkbuttonStartup;
		
		private global::Gtk.Label GtkLabelStartup;
		
		private global::Gtk.Frame frame3;
		
		private global::Gtk.Alignment GtkAlignment2;
		
		private global::Gtk.Table table2;
		
		private global::Gtk.ComboBox comboboxTheme;
		
		private global::Gtk.Label labelTheme;
		
		private global::Gtk.Label GtkLabel3;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment1;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Button buttonOpenLogFolder;
		
		private global::Gtk.ComboBox comboboxLogging;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label GtkLabel2;
		
		private global::Gtk.Frame frame4;
		
		private global::Gtk.Alignment GtkAlignment3;
		
		private global::Gtk.CheckButton checkbuttonTurnOff;
		
		private global::Gtk.Label GtkLabel5;
		
		private global::Gtk.Label labelRestartWarning;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget BlinkStickClient.SettingsWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "BlinkStickClient.SettingsWidget";
			// Container child BlinkStickClient.SettingsWidget.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(12));
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.checkbuttonStartup = new global::Gtk.CheckButton ();
			this.checkbuttonStartup.CanFocus = true;
			this.checkbuttonStartup.Name = "checkbuttonStartup";
			this.checkbuttonStartup.Label = global::Mono.Unix.Catalog.GetString ("Automatically start application when I log on to Windows");
			this.checkbuttonStartup.DrawIndicator = true;
			this.checkbuttonStartup.UseUnderline = true;
			this.GtkAlignment.Add (this.checkbuttonStartup);
			this.frame1.Add (this.GtkAlignment);
			this.GtkLabelStartup = new global::Gtk.Label ();
			this.GtkLabelStartup.Name = "GtkLabelStartup";
			this.GtkLabelStartup.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Windows Startup</b>");
			this.GtkLabelStartup.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabelStartup;
			this.vbox2.Add (this.frame1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame1]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame3 = new global::Gtk.Frame ();
			this.frame3.Name = "frame3";
			this.frame3.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame3.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.table2 = new global::Gtk.Table (((uint)(1)), ((uint)(2)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.comboboxTheme = global::Gtk.ComboBox.NewText ();
			this.comboboxTheme.WidthRequest = 150;
			this.comboboxTheme.Name = "comboboxTheme";
			this.table2.Add (this.comboboxTheme);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboboxTheme]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.labelTheme = new global::Gtk.Label ();
			this.labelTheme.Name = "labelTheme";
			this.labelTheme.Xalign = 0F;
			this.labelTheme.LabelProp = global::Mono.Unix.Catalog.GetString ("Theme");
			this.table2.Add (this.labelTheme);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2 [this.labelTheme]));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment2.Add (this.table2);
			this.frame3.Add (this.GtkAlignment2);
			this.GtkLabel3 = new global::Gtk.Label ();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>User Interface</b>");
			this.GtkLabel3.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel3;
			this.vbox2.Add (this.frame3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame3]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.buttonOpenLogFolder = new global::Gtk.Button ();
			this.buttonOpenLogFolder.CanFocus = true;
			this.buttonOpenLogFolder.Name = "buttonOpenLogFolder";
			this.buttonOpenLogFolder.UseUnderline = true;
			this.buttonOpenLogFolder.Label = global::Mono.Unix.Catalog.GetString ("Open Log Folder");
			this.table1.Add (this.buttonOpenLogFolder);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.buttonOpenLogFolder]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboboxLogging = global::Gtk.ComboBox.NewText ();
			this.comboboxLogging.AppendText (global::Mono.Unix.Catalog.GetString ("Off"));
			this.comboboxLogging.AppendText (global::Mono.Unix.Catalog.GetString ("Light"));
			this.comboboxLogging.AppendText (global::Mono.Unix.Catalog.GetString ("Full"));
			this.comboboxLogging.WidthRequest = 150;
			this.comboboxLogging.Name = "comboboxLogging";
			this.table1.Add (this.comboboxLogging);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboboxLogging]));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Logging Level:");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment1.Add (this.table1);
			this.frame2.Add (this.GtkAlignment1);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Logging</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel2;
			this.vbox2.Add (this.frame2);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame2]));
			w14.Position = 2;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame4 = new global::Gtk.Frame ();
			this.frame4.Name = "frame4";
			this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame4.Gtk.Container+ContainerChild
			this.GtkAlignment3 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment3.Name = "GtkAlignment3";
			this.GtkAlignment3.LeftPadding = ((uint)(12));
			// Container child GtkAlignment3.Gtk.Container+ContainerChild
			this.checkbuttonTurnOff = new global::Gtk.CheckButton ();
			this.checkbuttonTurnOff.CanFocus = true;
			this.checkbuttonTurnOff.Name = "checkbuttonTurnOff";
			this.checkbuttonTurnOff.Label = global::Mono.Unix.Catalog.GetString ("Turn off all BlinkSticks when exiting application");
			this.checkbuttonTurnOff.DrawIndicator = true;
			this.checkbuttonTurnOff.UseUnderline = true;
			this.GtkAlignment3.Add (this.checkbuttonTurnOff);
			this.frame4.Add (this.GtkAlignment3);
			this.GtkLabel5 = new global::Gtk.Label ();
			this.GtkLabel5.Name = "GtkLabel5";
			this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Devices</b>");
			this.GtkLabel5.UseMarkup = true;
			this.frame4.LabelWidget = this.GtkLabel5;
			this.vbox2.Add (this.frame4);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame4]));
			w17.Position = 3;
			w17.Expand = false;
			w17.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.labelRestartWarning = new global::Gtk.Label ();
			this.labelRestartWarning.Name = "labelRestartWarning";
			this.labelRestartWarning.LabelProp = global::Mono.Unix.Catalog.GetString ("<span foreground=\"red\">Please restart application for the changes to take effect<" +
			"/span>");
			this.labelRestartWarning.UseMarkup = true;
			this.vbox2.Add (this.labelRestartWarning);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.labelRestartWarning]));
			w18.Position = 5;
			w18.Expand = false;
			w18.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.labelRestartWarning.Hide ();
			this.Hide ();
			this.checkbuttonStartup.Toggled += new global::System.EventHandler (this.OnCheckbuttonStartupToggled);
			this.comboboxTheme.Changed += new global::System.EventHandler (this.OnComboboxThemeChanged);
			this.comboboxLogging.Changed += new global::System.EventHandler (this.OnComboboxLoggingChanged);
			this.buttonOpenLogFolder.Clicked += new global::System.EventHandler (this.OnButtonOpenLogFolderClicked);
			this.checkbuttonTurnOff.Toggled += new global::System.EventHandler (this.OnCheckbuttonTurnOffToggled);
		}
	}
}
