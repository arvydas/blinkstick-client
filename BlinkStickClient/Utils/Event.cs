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
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

//TODO: Fix badly ported code from WinForms

namespace BlinkStickClient.Utils
{
	public delegate void EventSignalledHandler();

	/// <summary>
	/// This implements single instance.  It turns out that single instance is
	/// fairly easy with either mutex or event.  The other part of the job is
	/// to get the first instance to display its window in the foreground, which
	/// is much more difficult, especially if the app window is initially hidden.
	/// this class should be created in main().  the first instance will create
	/// the event.  The second instance will open it, then signal it and exit.  The first
	/// event starts a thread which waits indefinitely for the event to signal.
	/// That thread has to call Invoke() so that the main thread can update the GUI.
	/// This class must implement IDisposable to ensure that the unmanaged Win32 handle is released
	/// </summary>
	public class Event : IDisposable
	{
		private bool disposed = false;

		[DllImport("kernel32.dll")] static extern uint WaitForSingleObject( IntPtr hHandle, uint dwMilliseconds );
		[DllImport("kernel32.dll")] static extern IntPtr CreateEvent( IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
		[DllImport("kernel32.dll")] static extern bool SetEvent( IntPtr hEvent );
		[DllImport("kernel32.dll")] static extern IntPtr OpenEvent( UInt32 dwDesiredAccess, bool bInheritable, string lpName );
		[DllImport("kernel32.dll")] static extern bool CloseHandle( IntPtr hHandle );
		[DllImport("kernel32.dll")] static extern bool ResetEvent( IntPtr hEvent );

		private const uint INFINITE				= 0xFFFFFFFF;
		private const uint SYNCHRONIZE			= 0x00100000;
		private const uint EVENT_MODIFY_STATE	= 0x0002;

		private IntPtr eventHandle = IntPtr.Zero;		// unmanaged
		private bool eventAlreadyExists = false;
        private EventSignalledHandler Handler;
		private Gtk.Window form;

        private Boolean CancelPending = false;

		// constructor
		public Event(String eventName)
		{
            eventHandle = OpenEvent(EVENT_MODIFY_STATE | SYNCHRONIZE, false, eventName);
			if ( eventHandle == IntPtr.Zero )
			{
                eventHandle = CreateEvent(IntPtr.Zero, true, false, eventName);
				if ( eventHandle != IntPtr.Zero )
				{
					Thread thread = new Thread( new ThreadStart( WaitForSignal ) );
					thread.Start();
				}
			}
			else
			{
				eventAlreadyExists = true;
			}

		}

		// destructor
		~Event()
		{
			Dispose( false );
		}

		// after creation, call this to determine if we are the first instance
		public bool EventAlreadyExists()
		{
			return eventAlreadyExists;
		}

		// an instance calls this when it detects that it is
		// the second instance.  Then it exits
		public void SignalEvent()
		{
			if ( eventHandle != IntPtr.Zero )
				SetEvent( eventHandle );
		}

		// the caller must give me a ref to its form, so that
		// we can do
		public void SetObject(Gtk.Window handlerForm, EventSignalledHandler handler)
		{
			Handler = handler;
            form = handlerForm;
		}

		// thread method will wait on the event, which will signal
		// if another instance tries to start
		private void WaitForSignal()
		{
            while (!CancelPending)
			{
				uint result = WaitForSingleObject( eventHandle, 1000 ); //INFINITE

				if ( result == 0 )
				{
					ResetEvent( eventHandle );
					Handler();
				}
				else
				{
					// what the heck, don't risk a busy loop
					// just let the thread die
					//break;
				}
			}
		}

        public void Cancel()
        {
            CancelPending = true;
        }

		#region IDisposable Members

		protected virtual void Dispose( bool disposeManagedResources )
		{
			if ( !this.disposed )
			{
				if ( disposeManagedResources )
				{
					// dispose managed resources
					if ( form != null )
					{
						form.Dispose();
						form = null;
					}
				}
				// dispose unmanaged resources
				if ( eventHandle != IntPtr.Zero )
					CloseHandle( eventHandle );
				eventHandle = IntPtr.Zero;

				disposed = true;
			}
		}


		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		#endregion
	}


    //***** Second method to detect single instance between different user accounts

    /// <summary>
    /// Summary description for ProcessInstance.
    /// </summary>
    public class ProcessInstance
    {
        /// <summary>
        /// Looks for other instances of the app that are already running
        /// </summary>
        /// <returns>a running process with the same name, if any</returns>
        public static Process GetRunningInstance()
        {
            // Get the current process and all processes with the same name
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //Loop through the running processes with the same name
            foreach (Process process in processes)
            {
                //Ignore the current process
                if (process.Id != current.Id)
                {
                    //Return the other process instance.
                    return process;
                }
            }

            return null;
        }
    }
}
