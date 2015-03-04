using System;
using log4net;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace BlinkStickClient.Utils
{
    public class FullScreenMonitor
    {
        private BackgroundWorker worker = null;

        private IntPtr WindowHandle;
        private Boolean FullScreen;

        #region Fields
        ILog log = LogManager.GetLogger("FullScreenMonitor");
        #endregion

        public event EventHandler<ChangedEventArgs> Changed;

        protected void OnChanged(uint processId, Boolean fullScreen)
        {
            if (Changed != null)
            {
                Changed(this, new ChangedEventArgs(processId, fullScreen));
            }
        }

        public FullScreenMonitor()
        {

        }

        #region Start/Stop
        public void Start()
        {
            log.Info("Starting...");

            worker = new BackgroundWorker();
            worker.DoWork += CheckActiveWindowThread;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        public void Stop()
        {
            worker.CancelAsync();
            log.Info("Stopped.");
        }
        #endregion

        void CheckActiveWindowThread (object sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
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

                if (!WindowHandle.Equals(hWnd) || FullScreen != runningFullScreen)
                {
                    WindowHandle = hWnd;
                    FullScreen = runningFullScreen;

                    uint processID = 0;
                    uint threadID = GetWindowThreadProcessId(hWnd, out processID);

                    log.DebugFormat("Change detected {0} (FullScreen:{1})", WindowHandle.ToInt64(), FullScreen);

                    OnChanged(processID, FullScreen);
                }

                DateTime last = DateTime.Now;
                TimeSpan difference;

                do
                {
                    difference = DateTime.Now - last;
                    Thread.Sleep(50);
                }
                while (difference.TotalSeconds >= 1 || worker.CancellationPending);
            }

            log.Info("Thread exited.");
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static IntPtr desktopHandle;
        private static IntPtr shellHandle;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow(); 

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow(); 

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }

    #region EventArgs subclasses
    public class ChangedEventArgs : EventArgs
    {
        public uint ProcessId;
        public Boolean FullScreen;

        public ChangedEventArgs(uint processId, Boolean fullScreen)
        {
            this.ProcessId = processId;
            this.FullScreen = fullScreen;
        }
    }
    #endregion

}

