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

        public Boolean UseThreadPool = true;
        #endregion

        #region Win32API
        Win32Api.WinEventDelegate dele;
        #endregion

        #region Start/Stop
        public void Start()
        {
            if (m_hhook == IntPtr.Zero)
            {
                log.Info("Starting ActiveWindowMonitor...");
                CheckTopLevelProcess();
                dele = new Win32Api.WinEventDelegate(WinEventProc);
                m_hhook = Win32Api.SetWinEventHook(Win32Api.EVENT_SYSTEM_FOREGROUND, Win32Api.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, Win32Api.WINEVENT_OUTOFCONTEXT);

                log.Info("ActiveWindowMonitor started");
            }
        }

        public void Stop()
        {
            if (m_hhook != IntPtr.Zero)
            {
                log.Info("Stopping ActiveWindowMonitor...");
                Win32Api.UnhookWinEvent(m_hhook);
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
            handle = Win32Api.GetForegroundWindow();

            uint processID = 0;
            uint threadID = Win32Api.GetWindowThreadProcessId(handle, out processID);

            if (ProcessChanged != null)
            {
                Process process = Process.GetProcessById((int)processID);

                ActiveProcess = ProcessExecutablePath(process);
            }
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
    #endregion
}
