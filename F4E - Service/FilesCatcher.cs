using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace F4E___Service
{
    class FilesCatcher
    {
        private static List<FileStream> fileStreams = new List<FileStream>();
        public static void CatchSystemFiles()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MMB");
            FilteringService.ShowMessage("CatchSystemFiles", path);
        }

        private static void CatchFile(FileInfo file)
        {     
            try
            {
                if (!file.FullName.Contains("SavedFilteringSettings") && !file.FullName.Contains("SavedScheduel"))
                {
                    File.SetAttributes(file.FullName, FileAttributes.ReadOnly | FileAttributes.Encrypted | FileAttributes.System | FileAttributes.Hidden);
                }

                fileStreams.Add(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            catch
            {
            }
        }

        public static void StopCatchingSystemFiles()
        {
            DirectoryInfo programFolder = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            foreach (FileInfo file in programFolder.GetFiles())
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
            }
            foreach (FileStream stream in fileStreams)
            {
                stream.Close();
            }

            DirectoryInfo appData = new DirectoryInfo(Program.GetAppDataFolder());
            foreach (FileInfo file in appData.GetFiles())
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
            }
            foreach (FileStream stream in fileStreams)
            {
                stream.Close();
            }


            //release hosts:
            FileInfo hosts = new FileInfo(Environment.SystemDirectory + @"\drivers\etc\hosts");
            File.SetAttributes(hosts.FullName, FileAttributes.Normal);
        }
    }
}
