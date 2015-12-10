
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.RadioAction OverviewAction;
	
	private global::Gtk.RadioAction NotificationsAction;
	
	private global::Gtk.RadioAction HelpAction1;
	
	private global::Gtk.RadioAction PatternsAction1;
	
	private global::Gtk.RadioAction EventsAction;
	
	private global::Gtk.VBox vbox2;
	
	private global::Gtk.VBox vbox3;
	
	private global::Gtk.HBox hbox1;
	
	private global::Gtk.VBox vbox4;
	
	private global::Gtk.Image imageBranding;
	
	private global::Gtk.Toolbar toolbar2;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.OverviewAction = new global::Gtk.RadioAction ("OverviewAction", global::Mono.Unix.Catalog.GetString ("Overview"), global::Mono.Unix.Catalog.GetString ("Overview"), "blinkstick-overview", 0);
		this.OverviewAction.Group = new global::GLib.SList (global::System.IntPtr.Zero);
		this.OverviewAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Overview");
		w1.Add (this.OverviewAction, null);
		this.NotificationsAction = new global::Gtk.RadioAction ("NotificationsAction", global::Mono.Unix.Catalog.GetString ("Notifications"), null, "blinkstick-notifications", 1);
		this.NotificationsAction.Group = this.OverviewAction.Group;
		this.NotificationsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Notifications");
		w1.Add (this.NotificationsAction, null);
		this.HelpAction1 = new global::Gtk.RadioAction ("HelpAction1", global::Mono.Unix.Catalog.GetString ("Help"), null, "blinkstick-help", 4);
		this.HelpAction1.Group = this.OverviewAction.Group;
		this.HelpAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
		w1.Add (this.HelpAction1, null);
		this.PatternsAction1 = new global::Gtk.RadioAction ("PatternsAction1", global::Mono.Unix.Catalog.GetString ("Patterns"), null, "blinkstick-patterns", 2);
		this.PatternsAction1.Group = this.HelpAction1.Group;
		this.PatternsAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Patterns");
		w1.Add (this.PatternsAction1, null);
		this.EventsAction = new global::Gtk.RadioAction ("EventsAction", global::Mono.Unix.Catalog.GetString ("Events"), null, "blinkstick-events", 3);
		this.EventsAction.Group = this.HelpAction1.Group;
		this.EventsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Events");
		w1.Add (this.EventsAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("BlinkStick");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox ();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		// Container child vbox2.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.vbox4 = new global::Gtk.VBox ();
		this.vbox4.Name = "vbox4";
		this.vbox4.Spacing = 6;
		// Container child vbox4.Gtk.Box+BoxChild
		this.imageBranding = new global::Gtk.Image ();
		this.imageBranding.Name = "imageBranding";
		this.vbox4.Add (this.imageBranding);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.imageBranding]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString (@"<ui><toolbar name='toolbar2'><toolitem name='OverviewAction' action='OverviewAction'/><toolitem name='NotificationsAction' action='NotificationsAction'/><toolitem name='PatternsAction1' action='PatternsAction1'/><toolitem name='EventsAction' action='EventsAction'/><toolitem name='HelpAction1' action='HelpAction1'/></toolbar></ui>");
		this.toolbar2 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar2")));
		this.toolbar2.Name = "toolbar2";
		this.toolbar2.Orientation = ((global::Gtk.Orientation)(1));
		this.toolbar2.ShowArrow = false;
		this.toolbar2.ToolbarStyle = ((global::Gtk.ToolbarStyle)(3));
		this.toolbar2.IconSize = ((global::Gtk.IconSize)(3));
		this.vbox4.Add (this.toolbar2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.toolbar2]));
		w3.Position = 1;
		this.hbox1.Add (this.vbox4);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox4]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		this.vbox3.Add (this.hbox1);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
		w5.Position = 0;
		this.vbox2.Add (this.vbox3);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.vbox3]));
		w6.Position = 0;
		this.Add (this.vbox2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 706;
		this.DefaultHeight = 584;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.OverviewAction.Toggled += new global::System.EventHandler (this.ToolbarButtonToggled);
		this.NotificationsAction.Toggled += new global::System.EventHandler (this.ToolbarButtonToggled);
		this.HelpAction1.Toggled += new global::System.EventHandler (this.ToolbarButtonToggled);
		this.PatternsAction1.Toggled += new global::System.EventHandler (this.ToolbarButtonToggled);
		this.EventsAction.Toggled += new global::System.EventHandler (this.ToolbarButtonToggled);
	}
}
