using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace F4E_design
{
    class ProblematicAppsBlocker
    {
         private static string[] blockedApp;
         private static int times = 599;

        public static void UpdateBlockedList()
        {
            if(InternetBlocker.IsInternetReachable())
                blockedApp = Regex.Split(Tools.GetTextFromUri("http://f4e.mmb.org.il/data/blockedApp"), "\r\n|\r|\n");
        }

        public static void Block()
        {
            //new Thread(() =>
            //{
                    times++;
                    if (times == 600)
                    {
                        UpdateBlockedList();
                        times = 0;
                    }
                    if (blockedApp != null)
                    {
                        foreach (string app in blockedApp)
                        {
                            CloseProcess(app);
                        }
                    }
            //}).Start();
        }

        public static void CloseProcess(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (Process process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch
                { }
            }
        }
    }
}
