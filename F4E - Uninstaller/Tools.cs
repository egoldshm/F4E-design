using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F4E___Uninstaller
{
    class Tools
    {
        public static void RunCmdCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
        }

        public static string LocateEXE(String filename)
        {
            String path = Environment.GetEnvironmentVariable("path");
            String[] folders = path.Split(';');
            foreach (String folder in folders)
            {
                if (System.IO.File.Exists(folder + filename))
                {
                    return folder;
                }
                else if (System.IO.File.Exists(folder + "\\" + filename))
                {
                    return folder;
                }
            }

            return String.Empty;
        }

    }
}
