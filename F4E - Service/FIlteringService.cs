using Microsoft.Win32;
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
    public partial class FilteringService : ServiceBase
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(IntPtr hServer, [MarshalAs(UnmanagedType.I4)] int SessionId, String pTitle, [MarshalAs(UnmanagedType.U4)] int TitleLength, String pMessage, [MarshalAs(UnmanagedType.U4)] int MessageLength, [MarshalAs(UnmanagedType.U4)] int Style, [MarshalAs(UnmanagedType.U4)] int Timeout, [MarshalAs(UnmanagedType.U4)] out int pResponse, bool bWait);

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public enum CustomCommends
        {
            startScheduelBlocking = 128,
            releaseScheduelBlocking = 129,
            kill = 131,
            setSafeDns = 132,
            setDHCPDns = 133,
            startCatchFiles = 134,
            stopCatchFiles = 135,
            updateHostsFile = 136,
            addToSafeMode = 137,
            removeFromSafeMode = 138,
            addToStartUp = 139,
            blockIncognitoMode=140,
            unblockIncognitoMode = 141,
        }

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1;
        public static Boolean allowSafemode = false;
        public static Boolean incognitoBlock = true;

        static Boolean msgShowed = false;

        System.Timers.Timer timer = new System.Timers.Timer();
        public FilteringService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 100; //number in milisecinds  
            timer.Start();
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            PreventClosing();
            IncognitoBlock(incognitoBlock);
            if(InternetBlocker.GetBlockStatus()==true)
            {
                if(InternetBlocker.IsInternetReachable())
                {
                    InternetBlocker.Block(true);
                }
            }
            else
            {
                if(!InternetBlocker.IsInternetReachable())
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
            IncognitoBlock(false);
            ShowMessage("Stopeed", "Stopped");
        }


        protected override void OnCustomCommand(int commend)
        {
            CustomCommends customCommend = (CustomCommends)commend;
            switch (customCommend)
            {
                case CustomCommends.startScheduelBlocking: //Interet Block Start
                    InternetBlocker.Block(true);
                    break;
                case CustomCommends.releaseScheduelBlocking: //Interet Block Stop
                    InternetBlocker.Block(false);
                    break;
                case CustomCommends.kill: //Kill Service
                    incognitoBlock = false;
                    Stop();
                    Application.Exit();
                    break;
                case CustomCommends.setSafeDns:
                    DnsController.SetMode(true);
                    break;
                case CustomCommends.setDHCPDns:
                    DnsController.SetMode(false);
                    break;
                case CustomCommends.startCatchFiles:
                    FilesCatcher.CatchSystemFiles();
                    break;
                case CustomCommends.stopCatchFiles:
                    FilesCatcher.StopCatchingSystemFiles();
                    break;
                case CustomCommends.updateHostsFile:
                    HostsFileAdapter.WriteCustomBlackListToHostFile();
                    break;
                case CustomCommends.addToSafeMode:
                    SafemodeAdapter.AddToSafeMode();
                    break;
                case CustomCommends.removeFromSafeMode:
                    SafemodeAdapter.RemoveFromSafeMode();
                    break;
                case CustomCommends.addToStartUp:
                    StartupAdapter.AddApplicationToAllUserStartup();
                    break;
            }
        }

        private void IncognitoBlock(Boolean mode)
        {
            int value = mode == true ? 1 : 0;
            try
            {
                RegistryKey key2;
                key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Google\Chrome", true);
                key2.SetValue("IncognitoModeAvailability", value.ToString());
            }
            catch { }
        }

        public static void ShowMessage(string title, string msg)
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
