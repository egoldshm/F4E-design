using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace F4E___Service
{
    class HostsFileAdapter
    {
        public static void WriteCustomBlackListToHostFile()
        {
            string hostFilePath = Environment.SystemDirectory + @"\drivers\etc\hosts";
            string blackListPath = Path.Combine(Program.GetAppDataFolder(), "MMB");
            try
            {
                File.WriteAllText(hostFilePath, File.ReadAllText(blackListPath));
            }
            catch(Exception e)
            {
                FilteringService.ShowMessage("Error Host Write", "hostFilePath: " + hostFilePath + Environment.NewLine + "BlackListFilePath: " + blackListPath + Environment.NewLine + e.Message);
            }
        }
    }
}
