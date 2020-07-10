using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace F4E___Service
{
    public partial class FIlteringService : ServiceBase
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(IntPtr hServer, [MarshalAs(UnmanagedType.I4)] int SessionId, String pTitle, [MarshalAs(UnmanagedType.U4)] int TitleLength, String pMessage, [MarshalAs(UnmanagedType.U4)] int MessageLength, [MarshalAs(UnmanagedType.U4)] int Style, [MarshalAs(UnmanagedType.U4)] int Timeout, [MarshalAs(UnmanagedType.U4)] out int pResponse, bool bWait);

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1;
        public static Boolean allowSafemode = false;

        Boolean msgShowed = false;

        System.Timers.Timer timer = new System.Timers.Timer();
        public FIlteringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 3500; //number in milisecinds  
            timer.Start();
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            PreventClosing();
            if(InternetBlocker.GetBlockStatus()==true)
            {
                if(InternetBlocker.IsInternetAvailable())
                {
                    InternetBlocker.Block(true);
                }
            }
            else
            {
                if(!InternetBlocker.IsInternetAvailable())
                {
                    InternetBlocker.Block(false);
                }
            }
        }

        Boolean processAlreadyDetected = false;

        private void PreventClosing()
        {
            if (!IsProcessOpen())
            {
                if (processAlreadyDetected)
                {
                    ShowMessage("F4E Filtering Sysetm", "The system recognized that the main process was unexpectedly shut down. PC REBOOT");
                    BootController.DoExitWin(BootController.EWX_REBOOT);
                }
            }
            else
            {
                processAlreadyDetected = true;
            }
        }

        private static void SetInterActWithDesktop()
        {
            var service = new System.Management.ManagementObject(
                    String.Format("WIN32_Service.Name='{0}'", "GUIAdapter"));
            try
            {
                var paramList = new object[11];
                paramList[5] = true;
                service.InvokeMethod("Change", paramList);
            }
            finally
            {
                service.Dispose();
            }


        }

        //private void RunGUIProcess()
        //{
        //    SetInterActWithDesktop();
        //    string processePath = Assembly.GetExecutingAssembly().CodeBase;
        //    processePath = processePath.Replace("F4E-Service.exe", "F4E by MMB.exe");
        //    Process.Start(processePath);
        //}

        private bool IsProcessOpen()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "F4E by MMB")
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnStop()
        {
            base.OnStop();
            timer.Stop();
            timer.Enabled = false;
            InternetBlocker.Block(false);
        }


        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                case 128:
                    //ShowMessage("F4E - Schedueling Internet Blocking", "Scheduled internet blocking has started.");
                    InternetBlocker.Block(true);
                    break;
                case 129:
                    //ShowMessage("F4E - Schedueling Internet Blocking", "Scheduled web blocking is over.");
                    InternetBlocker.Block(false);
                    break;
                case 131:
                    Stop();
                    Application.Exit();
                    break;
            }
        }
        private void ShowMessage(string title, string msg)
        {
            if (!msgShowed)
            {
                for (int user_session = 10; user_session > 0; user_session--)
                {
                    Thread t = new Thread(() =>
                    {
                        try
                        {
                            msgShowed = true;
                            bool result = false;
                            int tlen = title.Length;
                            int mlen = msg.Length;
                            int resp = 7;
                            result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, user_session, title, tlen, msg, mlen, 0, 0, out resp, true);
                            int err = Marshal.GetLastWin32Error();
                            if (err == 0)
                            {
                                if (result) //user responded to box
                                {
                                    //if (resp == 1) //user clicked ok
                                    //{

                                    //}
                                    msgShowed = false;
                                    Debug.WriteLine("user_session:" + user_session + " err:" + err + " resp:" + resp);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("no such thread exists", ex);
                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            }
        }
    }
}
