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
            string hostFilePath = Path.Combine(Environment.SystemDirectory, @"drivers\etc\hosts");
            File.WriteAllText(hostFilePath, File.ReadAllText(blackListPath));
        }
    }
}
