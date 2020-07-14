using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace F4E___Service
{
    class StartupAdapter
    {
     
        public static void AddApplicationToAllUserStartup()
        {
            try
            {
                Process process = Process.GetProcessesByName("F4E by MMB")[0];
                string path = process.MainModule.FileName.Replace("\"","");

                WshShell wsh = new WshShell();
                string shurtcutPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\StartUp\‏‏F4E by MMB.lnk";
                IWshShortcut shortcut = wsh.CreateShortcut(shurtcutPath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Arguments = "";
                shortcut.TargetPath = path;
                // not sure about what this is for
                shortcut.WindowStyle = 1;
                shortcut.Description = "F4E by MMB";
                //shortcut.WorkingDirectory = "c:\\app";
                shortcut.Save();
            }
            catch(Exception e)
            {
                FilteringService.ShowMessage("AddApplicationToAllUserStartup", "faild" + Environment.NewLine + e.Message);
            }
        }

      
        public static void RemoveApplicationFromAllUserStartup()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("F4E by MMB", false);
            }
        }
    }
}
