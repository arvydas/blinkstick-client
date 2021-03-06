
// This file has been generated by the GUI designer. Do not modify.
namespace BlinkStickClient
{
	public partial class OverviewWidget
	{
		private global::Gtk.VBox vbox4;
		
		private global::Gtk.HBox hboxMiniMenu;
		
		private global::Gtk.Label labelSelectBlinkStick;
		
		private global::Gtk.Button buttonRefresh;
		
		private global::Gtk.Button buttonConfigure;
		
		private global::Gtk.Button buttonDelete;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment8;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label GtkLabel11;
		
		private global::Gtk.HSeparator hseparator1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget BlinkStickClient.OverviewWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "BlinkStickClient.OverviewWidget";
			// Container child BlinkStickClient.OverviewWidget.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hboxMiniMenu = new global::Gtk.HBox ();
			this.hboxMiniMenu.Name = "hboxMiniMenu";
			this.hboxMiniMenu.Spacing = 6;
			this.hboxMiniMenu.BorderWidth = ((uint)(12));
			// Container child hboxMiniMenu.Gtk.Box+BoxChild
			this.labelSelectBlinkStick = new global::Gtk.Label ();
			this.labelSelectBlinkStick.Name = "labelSelectBlinkStick";
			this.labelSelectBlinkStick.LabelProp = global::Mono.Unix.Catalog.GetString ("Select BlinkStick:");
			this.hboxMiniMenu.Add (this.labelSelectBlinkStick);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hboxMiniMenu [this.labelSelectBlinkStick]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hboxMiniMenu.Gtk.Box+BoxChild
			this.buttonRefresh = new global::Gtk.Button ();
			this.buttonRefresh.CanFocus = true;
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.UseUnderline = true;
			global::Gtk.Image w2 = new global::Gtk.Image ();
			w2.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("BlinkStickClient.Resources.icons.icon-dark-refresh.png");
			this.buttonRefresh.Image = w2;
			this.hboxMiniMenu.Add (this.buttonRefresh);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hboxMiniMenu [this.buttonRefresh]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hboxMiniMenu.Gtk.Box+BoxChild
			this.buttonConfigure = new global::Gtk.Button ();
			this.buttonConfigure.CanFocus = true;
			this.buttonConfigure.Name = "buttonConfigure";
			this.buttonConfigure.UseUnderline = true;
			global::Gtk.Image w4 = new global::Gtk.Image ();
			w4.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("BlinkStickClient.Resources.icons.icon-dark-cog-small.png");
			this.buttonConfigure.Image = w4;
			this.hboxMiniMenu.Add (this.buttonConfigure);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hboxMiniMenu [this.buttonConfigure]));
			w5.Position = 3;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hboxMiniMenu.Gtk.Box+BoxChild
			this.buttonDelete = new global::Gtk.Button ();
			this.buttonDelete.CanFocus = true;
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.UseUnderline = true;
			global::Gtk.Image w6 = new global::Gtk.Image ();
			w6.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("BlinkStickClient.Resources.icons.icon-dark-trash.png");
			this.buttonDelete.Image = w6;
			this.hboxMiniMenu.Add (this.buttonDelete);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hboxMiniMenu [this.buttonDelete]));
			w7.Position = 4;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox4.Add (this.hboxMiniMenu);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hboxMiniMenu]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment8 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment8.Name = "GtkAlignment8";
			this.GtkAlignment8.LeftPadding = ((uint)(12));
			// Container child GtkAlignment8.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.GtkAlignment8.Add (this.hbox1);
			this.frame2.Add (this.GtkAlignment8);
			this.GtkLabel11 = new global::Gtk.Label ();
			this.GtkLabel11.Name = "GtkLabel11";
			this.GtkLabel11.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Change Color</b>");
			this.GtkLabel11.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel11;
			this.vbox4.Add (this.frame2);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.frame2]));
			w11.Position = 2;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.vbox4.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator1]));
			w12.Position = 3;
			w12.Expand = false;
			w12.Fill = false;
			this.Add (this.vbox4);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.buttonRefresh.Clicked += new global::System.EventHandler (this.OnButtonRefreshClicked);
			this.buttonConfigure.Clicked += new global::System.EventHandler (this.OnButtonConfigureClicked);
			this.buttonDelete.Clicked += new global::System.EventHandler (this.OnButtonDeleteClicked);
		}
	}
}
