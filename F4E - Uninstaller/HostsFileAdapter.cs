using System.IO;

namespace F4E___Uninstaller
{
    class HostsFileAdapter
    {
        private static readonly string HOSTS_FILE_PATH = @"C:\Windows\System32\drivers\etc\hosts";

        public static void ClearAll()
        {
            File.SetAttributes(HOSTS_FILE_PATH, FileAttributes.Normal);
            File.WriteAllText(HOSTS_FILE_PATH, "");
        }
    }
}
