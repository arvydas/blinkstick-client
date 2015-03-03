using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace BlinkStickClient.Utils
{
    public class ActiveWindowMonitor
    {
        #region Events
        public event EventHandler<ProcessChangedEventArgs> ProcessChanged;

        protected void OnProcessChanged(String executableFileName)
        {
            log.DebugFormat("Active process changed to {0} ", executableFileName);
            if (ProcessChanged != null)
            {
                ProcessChanged(this, new ProcessChangedEventArgs(executableFileName));
            }
        }

        public event EventHandler<ProcessIdChangedEventArgs> ProcessIdChanged;

        protected void OnProcessIdChanged(uint processId, Boolean fullScreen)
        {
            log.DebugFormat("Active process id changed to {0}, fullScreen:{1}", processId, fullScreen);
            if (ProcessIdChanged != null)
            {
                ProcessIdChanged(this, new ProcessIdChangedEventArgs(processId, fullScreen));
            }
        }
        #endregion

        #region Fields
        ILog log = LogManager.GetLogger("ActiveWindowMonitor");
        IntPtr m_hhook = IntPtr.Zero;
        #endregion

        #region Properties
        private String _ActiveProcess;

        public String ActiveProcess
        {
            get { return _ActiveProcess; }
            private set 
            {
                if (_ActiveProcess != value)
                {
                    _ActiveProcess = value;
                    OnProcessChanged(value);
                }
            }
        }

        private uint _ActiveProcessId;

        public uint ActiveProcessId
        {
            get { return _ActiveProcessId; }
            private set 
            {
                if (_ActiveProcessId != value)
                {
                    _ActiveProcessId = value;
                }
            }
        }

        public Boolean UseThreadPool = true;
        #endregion

        #region Win32API
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        WinEventDelegate dele;

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int smIndex);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        #region Start/Stop
        public void Start()
        {
            if (m_hhook == IntPtr.Zero)
            {
                log.Info("Starting ActiveWindowMonitor...");
                CheckTopLevelProcess();
                dele = new WinEventDelegate(WinEventProc);
                m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

                log.Info("ActiveWindowMonitor started");
            }
        }

        public void Stop()
        {
            if (m_hhook != IntPtr.Zero)
            {
                log.Info("Stopping ActiveWindowMonitor...");
                UnhookWinEvent(m_hhook);
                m_hhook = IntPtr.Zero;
                log.Info("ActiveWindowMonitor stopped");
            }
        }
        #endregion

        #region Top Level Window Checker Implementation
        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (UseThreadPool)
            {
                ThreadPool.QueueUserWorkItem((_) => CheckTopLevelProcess());
            }
            else
            {
                CheckTopLevelProcess();
            }
        }

        private void CheckTopLevelProcess()
        {
            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();

            uint processID = 0;
            uint threadID = GetWindowThreadProcessId(handle, out processID);

            if (ProcessIdChanged != null)
            {
                Boolean fullScreen = false;

                int scrX = GetSystemMetrics(SM_CXSCREEN),
                scrY = GetSystemMetrics(SM_CYSCREEN);

                RECT wRect;
                if (GetWindowRect(handle, out wRect))
                {
                    log.DebugFormat("{0}:{1} {2},{3},{4},{5}", scrX, scrY, wRect.Right, wRect.Left, wRect.Bottom, wRect.Top);
                    fullScreen = scrX == (wRect.Right - wRect.Left) && scrY == (wRect.Bottom - wRect.Top);
                }

                //if (ActiveProcessId != processID)
                {
                    ActiveProcessId = processID;
                    //OnProcessIdChanged(processID, fullScreen);
                    OnProcessIdChanged(processID, AreApplicationFullScreen());
                }
            }

            if (ProcessChanged != null)
            {
                Process process = Process.GetProcessById((int)processID);

                ActiveProcess = ProcessExecutablePath(process);
            }
        }

        private static IntPtr desktopHandle;
        private static IntPtr shellHandle;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow(); 

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow(); 

        public static bool AreApplicationFullScreen() {
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();

            bool runningFullScreen = false;
            RECT appBounds;
            System.Drawing.Rectangle screenBounds;
            IntPtr hWnd;
            hWnd = GetForegroundWindow();

            if (hWnd != null && !hWnd.Equals(IntPtr.Zero)) {
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle))) {
                    GetWindowRect(hWnd, out appBounds);
                    screenBounds = System.Windows.Forms.Screen.FromHandle(hWnd).Bounds;
                    if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height
                        && (appBounds.Right - appBounds.Left) == screenBounds.Width) {
                        runningFullScreen = true;
                    }
                }
            }

            return runningFullScreen;
        } 

        private string ProcessExecutablePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {

                try
                {
                    string query = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessID=" + process.Id.ToString();
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                    {
                        searcher.Options.Timeout = new TimeSpan(0, 0, 0, 10, 0);
                        searcher.Options.ReturnImmediately = false;
                        foreach (ManagementObject item in searcher.Get())
                        {
                            object path = item["ExecutablePath"];

                            if (path != null)
                            {
                                return path.ToString();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Attempt to get ExecutablePath failed: {0}", e.Message);
                }
            }

            return "";
        }
        #endregion
    }

    #region EventArgs subclasses
    public class ProcessChangedEventArgs : EventArgs
    {
        public String ExecutableFileName;

        public ProcessChangedEventArgs(String executableFileName)
        {
            ExecutableFileName = executableFileName;
        }
    }

    public class ProcessIdChangedEventArgs : EventArgs
    {
        public uint ProcessId;
        public Boolean FullScreen;

        public ProcessIdChangedEventArgs(uint processId, Boolean fullScreen)
        {
            this.ProcessId = processId;
            this.FullScreen = fullScreen;
        }
    }
    #endregion
}
