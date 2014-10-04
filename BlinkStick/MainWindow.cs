#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.IO;
using BlinkStick;
using Gdk;
using Gtk;
using log4net;
using BlinkStickClient;
using BlinkStickClient.Classes;
using BlinkStickDotNet;
using BlinkStickClient.Utils;
using MonoDevelop.MacInterop;
#if LINUX
using AppIndicator;
#endif

public partial class MainWindow: Gtk.Window
{	
	protected static readonly ILog log = LogManager.GetLogger("Main");	

	private static BlinkStickClient.Utils.Event instanceExistsEvent;
    public BlinkStickClient.Utils.EventSignalledHandler eventSignalled;

	Gtk.ListStore EventListStore = new ListStore(typeof(CustomNotification));

	NotificationManager Manager;
	Boolean IgnoreActivation = false;

	private Boolean ApplicationIsClosing = false;
#if LINUX
	private ApplicationIndicator indicator;	
#else
	private StatusIcon trayIcon;
#endif
	UsbMonitor DeviceMonitor;
	BlinkStickTestForm testForm;

	private Menu popupMenu;
	public CustomNotification _SelectedNotification = null;

	public CustomNotification SelectedNotification {
		get {
			return _SelectedNotification;
		}
		set {
			if (_SelectedNotification != value)
			{
				_SelectedNotification = value;
				UpdateButtons();
			}
		}
	}

	private String _ExecutableFolder = "";
	private String ExecutableFolder {
		get {
			if (_ExecutableFolder == "")
				_ExecutableFolder = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

			return _ExecutableFolder;
		}
	}

	private static String _ApplicationVersion = "";
	public static String ApplicationVersion {
		get {
			if (_ApplicationVersion == "")
			{
				_ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
				_ApplicationVersion = _ApplicationVersion.Substring (0, _ApplicationVersion.LastIndexOf ('.'));
			}
			return _ApplicationVersion;
		}
	}

    public static String LogFolder
    {
        get
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Agile Innovative", "BlinkStick", "logs");
        }
    }

    public static String LogFile
    {
        get
        {
            return System.IO.Path.Combine(LogFolder, "BlinkStick.log");
        }
    }
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
        if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows)
		{
			SetupSingleInstanceEvent();
		}

		Build ();

		this.Title = "BlinkStick " + ApplicationVersion;

		log.Debug("Setting up controls");

		Gtk.TreeViewColumn eventTitleColumn = new Gtk.TreeViewColumn ();
		eventTitleColumn.Title = "Name";

		Gtk.TreeViewColumn eventTypeColumn = new Gtk.TreeViewColumn ();
		eventTypeColumn.Title = "Type";

		Gtk.TreeViewColumn eventColorColumn = new Gtk.TreeViewColumn ();
		eventColorColumn.Title = "Color";

		// Create the text cell that will display the artist name
		Gtk.CellRendererText eventTitleCell = new Gtk.CellRendererText ();
		Gtk.CellRendererText eventTypeCell = new Gtk.CellRendererText ();
		Gtk.CellRendererPixbuf eventColorCell = new Gtk.CellRendererPixbuf ();

		log.Debug ("Loading main form icon");
		this.Icon = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
			 
		log.Debug( "Setting up treeview");
		// Add the cell to the column
		eventColorColumn.PackStart (eventColorCell, false);
		eventTypeColumn.PackEnd (eventTypeCell, true);
		eventTitleColumn.PackEnd (eventTitleCell, true);
	 
		// Tell the Cell Renderers which items in the model to display
		//eventTitleColumn.AddAttribute (eventTitleCell, "name", 0);
		eventTitleColumn.SetCellDataFunc (eventTitleCell, new Gtk.TreeCellDataFunc (RenderEventName));
		eventTypeColumn.SetCellDataFunc (eventTypeCell, new Gtk.TreeCellDataFunc (RenderEventType));
		eventColorColumn.SetCellDataFunc (eventColorCell, new Gtk.TreeCellDataFunc (RenderEventColor));

		treeviewEvents.Model = EventListStore;

		treeviewEvents.AppendColumn (eventColorColumn);
		treeviewEvents.AppendColumn (eventTypeColumn);
		treeviewEvents.AppendColumn (eventTitleColumn);

		log.Debug("Updating buttons");
		UpdateButtons ();

		log.Debug("Setting up notification manager");
		Manager = new NotificationManager ();
		Manager.NotificationUpdated += HandleNotificationUpdated;
		Manager.Load ();
		Manager.UpdateControllers();

		DeviceMonitor = new UsbMonitor();
        DeviceMonitor.UsbDevicesChanged += (object sender, EventArgs e) => {
			Gtk.Application.Invoke (delegate {
				Manager.UpdateControllers();

				if (testForm != null)
				{
					testForm.PopulateForm();
				}
			});
		};
		DeviceMonitor.Start ();

		log.Debug("Adding notifications to the tree");
		//Gtk.TreeIter customEventRoot = EventListStore.AppendValues(new TriggeredEvent("Custom"));
		foreach (CustomNotification e in Manager.Notifications) {
			//EventListStore.AppendValues(customEventRoot, e);
			EventListStore.AppendValues (e);
		}

		log.Debug("Starting notification manager");
		Manager.Start ();

		log.Debug ("Building popup menu");
		//Build Popup Menu for TrayIcon
		popupMenu = new Menu ();

		//Settings menu item
		ImageMenuItem menuItemSettings = new ImageMenuItem ("Settings");
		menuItemSettings.Image = new Gtk.Image(Stock.Preferences, IconSize.Menu);
		menuItemSettings.Activated += ToggleMainWindow;
		popupMenu.Append(menuItemSettings);

		popupMenu.Append(new SeparatorMenuItem());

		//Quit menu item
		ImageMenuItem menuItemQuit = new ImageMenuItem ("Quit");
		menuItemQuit.Image = new Gtk.Image (Stock.Quit, IconSize.Menu);
		menuItemQuit.Activated += OnQuitActionActivated;
		popupMenu.Append (menuItemQuit);

		log.Debug("Showing popup menu");
		popupMenu.ShowAll();
        //TODO: Remove ifdef and use platform detection
#if LINUX
		indicator = new ApplicationIndicator ("blinkstick", "icon", Category.ApplicationStatus, ExecutableFolder);	
		indicator.Menu = popupMenu;
		indicator.Status = AppIndicator.Status.Active;	
#else

		log.Debug ("Setting up tray icon");
		trayIcon = new StatusIcon (new Pixbuf (System.IO.Path.Combine(ExecutableFolder, "icon.ico")));
		trayIcon.Tooltip = this.Title;
		trayIcon.Visible = true;

		// Show/Hide the window (even from the Panel/Taskbar) when the TrayIcon has been clicked.
		trayIcon.Activate += ToggleMainWindow;

		trayIcon.PopupMenu += delegate {
			popupMenu.ShowAll ();
			popupMenu.Popup ();
		};
#endif
        if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Mac) {
            //enable the global key handler for keyboard shortcuts
            MacMenu.GlobalKeyHandlerEnabled = true;

            //Tell the IGE library to use your GTK menu as the Mac main menu
            MacMenu.MenuBar = menubar2;

            //tell IGE which menu item should be used for the app menu's quit item
            MacMenu.QuitMenuItem = menuItemQuit;

            //add a new group to the app menu, and add some items to it
            var appGroup = MacMenu.AddAppMenuGroup ();
            appGroup.AddMenuItem (menuItemQuit, "About BlinkStick Client...");
            appGroup.AddMenuItem (menuItemQuit, "Preferences...");

            //hide the menu bar so it no longer displays within the window
            menubar2.Hide ();


            ApplicationEvents.Quit += delegate(object sender, ApplicationQuitEventArgs e)
            {
                OnQuitActionActivated(null, null);
                e.Handled = true;
            };

            ApplicationEvents.Reopen += delegate (object sender, ApplicationEventArgs e) {
                this.Deiconify ();
                this.Visible = true;
                e.Handled = true;
            };

            //optional, only need this if your Info.plist registers to handle urls
            ApplicationEvents.OpenUrls += delegate (object sender, ApplicationUrlEventArgs e) {
                if (e.Urls != null || e.Urls.Count > 0) {
                    //OpenUrls (e.Urls);
                }
                e.Handled = true;
            };

        }

		log.Debug("Initialization done");
	}

	private void ToggleMainWindow (object sender, EventArgs e)
	{
		this.Visible = !this.Visible; 
	}

	void HandleNotificationUpdated (object sender, NotificationManager.NotificationUpdatedEventArgs e)
	{
		Gtk.Application.Invoke (delegate {
			TreeIter iter;
			Boolean searchMore = EventListStore.GetIterFirst(out iter);
			while (searchMore)
			{
				if (EventListStore.GetValue(iter, 0) == e.Notification)
				{
					EventListStore.EmitRowChanged(EventListStore.GetPath(iter), iter);
				}

				searchMore = EventListStore.IterNext(ref iter);
			}
		});
	}

	private void RenderEventName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		if (model.GetValue (iter, 0) is CustomNotification) {
			CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = tevent.Name;
		}
	}

	private void RenderEventType (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		if (model.GetValue (iter, 0) is CustomNotification) {
			CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = tevent.GetTypeName();
		}
	}

	private void RenderEventColor (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		if (model.GetValue (iter, 0) is CustomNotification) {
			CustomNotification tevent = (CustomNotification)model.GetValue (iter, 0);
			Gdk.Pixbuf pb = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, false, 8, 16, 16);
			pb.Fill ((uint)(0xff + tevent.Color.R * 0x1000000 + tevent.Color.G * 0x10000 + tevent.Color.B * 0x100));
			(cell as Gtk.CellRendererPixbuf).Pixbuf = pb;
		}
	}

	private void DestroyEnvironment ()
	{
		DeviceMonitor.Stop ();
		
		Manager.Stop ();
		Manager.Save ();

#if !LINUX
		trayIcon.Visible = false;
#endif

        if (HidSharp.PlatformDetector.RunningPlatform() == HidSharp.PlatformDetector.Platform.Windows)
		{
			instanceExistsEvent.Cancel();
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
#if DEBUG
		ApplicationIsClosing = true;
#endif 
		if (ApplicationIsClosing) {
			DestroyEnvironment();

			Application.Quit ();
		}

		a.RetVal = true;
		this.Hide();
	}

	protected void OnTreeviewEventsRowActivated (object o, RowActivatedArgs args)
	{
		EditNotificationForm.ShowForm(SelectedNotification, Manager);
	}

	private void UpdateButtons()
	{
		editAction.Sensitive = SelectedNotification != null;
		deleteAction.Sensitive = SelectedNotification != null;
		//checkAction.Sensitive = SelectedNotification != null;
		copyAction.Sensitive = SelectedNotification != null;
		activeAction.Sensitive = SelectedNotification != null;
		IgnoreActivation = true;
		activeAction.Active = SelectedNotification != null && SelectedNotification.Active;
		IgnoreActivation = false;
	}

	protected void OnTreeviewEventsCursorChanged (object sender, EventArgs e)
	{
		TreeModel model;
		TreeIter iter;

		TreeSelection selection = (sender as TreeView).Selection;

		if(selection.GetSelected(out model, out iter)){
			SelectedNotification = (CustomNotification)model.GetValue (iter, 0);
		}
	}

	protected void OnNewActionActivated (object sender, EventArgs e)
	{
		CustomNotification newEvent = SelectNotificationTypeForm.ShowForm();

		if (newEvent != null && EditNotificationForm.ShowForm(newEvent, Manager))
		{
			EventListStore.AppendValues(newEvent);
			Manager.AddNotification (newEvent);
			newEvent.InitializeServices();
			Manager.Save();
		}
	}

	protected void OnDeleteActionActivated (object sender, EventArgs e)
	{
		TreeModel modelx;
		TreeIter iter;

		TreeSelection selection = treeviewEvents.Selection;

		if(selection.GetSelected(out modelx, out iter)){
			SelectedNotification.FinalizeServices();
			Manager.RemoveNotification(SelectedNotification);
			EventListStore.Remove(ref iter);
			Manager.Save();
		}
	}

	protected void OnEditActionActivated (object sender, EventArgs e)
	{
		EditNotificationForm.ShowForm(SelectedNotification, Manager);
		Manager.Save();
	}

	protected void OnTestActionActivated (object sender, EventArgs e)
	{
		testForm = new BlinkStickTestForm ();
		testForm.Manager = this.Manager;
		testForm.PopulateForm();
		testForm.Run ();
		testForm.Destroy();
		testForm = null;

		//BlinkStickTestForm.ShowForm(Manager);
	}

	protected void OnCopyActionActivated (object sender, EventArgs e)
	{
		CustomNotification ev = SelectedNotification.Copy();
		if (EditNotificationForm.ShowForm(ev, Manager))
		{
			EventListStore.AppendValues(ev);
			Manager.AddNotification (ev);
			ev.InitializeServices();
		}
	}

	protected void OnActiveActionToggled (object sender, EventArgs e)
	{
		if (IgnoreActivation)
			return;

		SelectedNotification.Active = activeAction.Active;
	}
	protected void OnQuitActionActivated (object sender, EventArgs e)
	{
		ApplicationIsClosing = true;
		DestroyEnvironment();
        Gtk.Application.Quit ();
	}

	protected void OnCheckActionChanged (object o, ChangedArgs args)
	{
		SelectedNotification.Check ();
	}

	protected void OnManageActionActivated (object sender, EventArgs e)
	{
		BlinkStickManageForm.ShowForm(Manager);
	}

	protected void OnHideActionActivated (object sender, EventArgs e)
	{
		this.Hide ();
	}

	protected void OnSupportActionActivated (object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Process.Start("http://www.blinkstick.com/help");
		}
		catch
		{
		}
	}

	protected void OnReportABugActionActivated (object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Process.Start("http://blinkstick.zendesk.com");
		}
		catch
		{
		}
	}

	protected void OnAboutActionActivated (object sender, EventArgs e)
	{
		BlinkStickClient.AboutDialog.ShowDialog(this.Title);
	}


	protected void OnOpenLogActionActivated (object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Process.Start("notepad", LogFile);
		}
		catch
		{
		}
	}

	
	#region Singleton enforcment methods
	private void SetupSingleInstanceEvent()
	{
		// this object provides single instance feature
		instanceExistsEvent = new BlinkStickClient.Utils.Event("Local\\blinkstick_client");
		
		// already exists means another instance is running for the same user
		if (instanceExistsEvent.EventAlreadyExists())
		{
			// signal the other instance to come to foreground
			instanceExistsEvent.SignalEvent();
			Environment.Exit(0);
		}
		/* TODO: check for instances run by another user
		else if (ProcessInstance.GetRunningInstance() != null)
		{
			//MessageBox.Show("Application is already run by another user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Environment.Exit(0);
		}
		*/
		else
		{
			eventSignalled = new BlinkStickClient.Utils.EventSignalledHandler(evnt_EventSignalled);
			instanceExistsEvent.SetObject(this, eventSignalled);
		}
	}
	
	public void evnt_EventSignalled()
	{
		this.Visible = true;
	} 
	#endregion

}



