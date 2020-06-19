﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace F4ESERVICE
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(IntPtr hServer, [MarshalAs(UnmanagedType.I4)] int SessionId, String pTitle, [MarshalAs(UnmanagedType.U4)] int TitleLength, String pMessage, [MarshalAs(UnmanagedType.U4)] int MessageLength, [MarshalAs(UnmanagedType.U4)] int Style, [MarshalAs(UnmanagedType.U4)] int Timeout, [MarshalAs(UnmanagedType.U4)] out int pResponse, bool bWait);

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1;

        Boolean msgShowed = false;

        System.Timers.Timer timer = new System.Timers.Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 3500; //number in milisecinds  
            timer.Enabled = true;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            PreventClosing();
            if (InternetBlocker.GetBlockStatus() == true)
            {
                if (InternetBlocker.IsInternetAvailable())
                {
                    InternetBlocker.Block(true);
                }
            }
            else
            {
                if (!InternetBlocker.IsInternetAvailable())
                {
                    InternetBlocker.Block(false);
                }
            }
        }

        int count = 0;
        private void PreventClosing()
        {
            if (!IsProcessOpen())
            {
                //if (InternetBlocker.IsInternetAvailable())
                //{
                //    ShowMessage("11", "22");
                //    InternetBlocker.Block(true);
                //}

                ShotdownPC();

                if (count % 15 == 0 && count > 3)
                {
                    ShowMessage("MMB Filtering Sysetm", "The system recognized that the main process was unexpectedly shut down. The Internet is disabled until the filtering system will be restart.");
                }
                count++;
            }
            else
            {
                count = 15;
            }
        }

        private void ShotdownPC()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                Arguments = @"shutdown -s -t 0"
            };
            ShowMessage("!23", "!23");
            Process.Start(processInfo);
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
            timer.Stop();
            timer.Enabled = false;
            InternetBlocker.Block(false);
        }


        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                case 128:
                    InternetBlocker.Block(true);
                    break;
                case 129:
                    InternetBlocker.Block(false);
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