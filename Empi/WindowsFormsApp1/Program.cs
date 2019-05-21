using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool isAlreadyRunning = IsAlreadyRunning();
            Form1 form1 = new Form1();
            if (args.Length == 1)
            {
                Uri uri = new Uri(args[0]);
                NameValueCollection nameValues = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string firstName = nameValues.Get("firstName");
                string lastName = nameValues.Get("lastName");
                string dobMonth = nameValues.Get("dobMonth");
                string dobDay = nameValues.Get("dobDay");
                string dobYear = nameValues.Get("dobYear");
                form1 = new Form1(firstName, lastName, dobMonth, dobDay, dobYear, uri.OriginalString, isAlreadyRunning);
            }
            Process currentProcess = GetCurrentProcess();
            if (currentProcess != null)
            {   //There is another one running, close it
                //Form currentForm = (Form)Form.FromHandle(currentProcess.MainWindowHandle);
                currentProcess.CloseMainWindow();
                currentProcess.WaitForExit();
            }
            //if (isAlreadyRunning)
            //{
            //    IntPtr wHnd = GetCurrentInstanceWindowHandle();
            //    if (wHnd != IntPtr.Zero)
            //    {
            //        SwitchToCurrentInstance(wHnd);
            //        return;
            //    }
            //}
            Application.Run(form1);
        }
        /// <summary>
        /// check if given exe already running or not
        /// </summary>
        /// <returns>returns true if already running</returns>
        private static bool IsAlreadyRunning()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            FileSystemInfo fileInfo = new FileInfo(location);
            string exeName = fileInfo.Name;

            mutex = new Mutex(true, "Global\\" + exeName, out bool newlyCreated);
            if (newlyCreated)
            {
                mutex.ReleaseMutex();
            }

            return !newlyCreated;
        }

        static Mutex mutex;
        const int SW_RESTORE = 9;

        /// <summary>
        /// GetCurrentInstanceWindowHandle
        /// </summary>
        /// <returns></returns>
        private static IntPtr GetCurrentInstanceWindowHandle()
        {
            IntPtr hWnd = IntPtr.Zero;
            Process newProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(newProcess.ProcessName);
            foreach (Process process in processes)
            {
                // Get the first instance that is not this instance, has the same process name and was started from the same file name and location. 
                //Also check that the process has a valid window handle in this session to filter out other user's processes.
                if (process.Id != newProcess.Id &&
                    process.MainModule.FileName == newProcess.MainModule.FileName &&
                    process.MainWindowHandle != IntPtr.Zero)
                {
                    hWnd = process.MainWindowHandle;
                    break;
                }
            }
            return hWnd;
        }

        private static Process GetCurrentProcess()
        {
            Process currentProcess = null;
            Process newProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(newProcess.ProcessName);
            foreach (Process process in processes)
            {
                // Get the first instance that is not this instance, has the same process name and was started from the same file name and location. 
                //Also check that the process has a valid window handle in this session to filter out other user's processes.
                if (process.Id != newProcess.Id &&
                    process.MainModule.FileName == newProcess.MainModule.FileName &&
                    process.MainWindowHandle != IntPtr.Zero)
                {
                    currentProcess = process;
                    break;
                }
            }
            return currentProcess;
        }

        /// <summary>
        /// SwitchToCurrentInstance
        /// </summary>
        private static void SwitchToCurrentInstance(IntPtr currentWindowHandle)
        {
            // Restore window if minimized. Do not restore if already in
            // normal or maximized window state, since we don't want to
            // change the current state of the window.
            if (IsIconic(currentWindowHandle) != 0)
            {
                ShowWindow(currentWindowHandle, SW_RESTORE);
            }

            // Set foreground window.
            SetForegroundWindow(currentWindowHandle);
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int IsIconic(IntPtr hWnd);
    }
}
