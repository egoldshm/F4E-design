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
            string blackListPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MMB"), "CustomBlackList");

            var OSInfo = Environment.OSVersion;
            string pathpart = "hosts";
            if (OSInfo.Platform == PlatformID.Win32NT)
            {
                //is windows NT
                pathpart = "system32\\drivers\\etc\\hosts";
            }
            string hostFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), pathpart);
            File.WriteAllText(hostFilePath, File.ReadAllText(blackListPath));
        }
    }
}
