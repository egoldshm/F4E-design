using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace F4E___Service
{
    class FilesCatcher
    {
        private static List<FileStream> fileStreams;
        public static void CatchSystemFiles()
        {
            fileStreams = new List<FileStream>();
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MMB");
            string exePath = System.AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo dataDirectory = new DirectoryInfo(dataPath);
            foreach (FileInfo file in dataDirectory.GetFiles())
            {
                CatchFile(file);
            }

            DirectoryInfo exeFolder = new DirectoryInfo(exePath);
            foreach (FileInfo file in exeFolder.GetFiles())
            {
                CatchFile(file);
            }
        }

        private static void CatchFile(FileInfo file)
        {     
            try
            {
                if (!file.FullName.Contains("SavedFilteringSettings") && !file.FullName.Contains("SavedScheduel") && !file.FullName.Contains("CustomBlackList") && !file.FullName.Contains("F4E by MMB"))
                {
                    File.SetAttributes(file.FullName, FileAttributes.ReadOnly | FileAttributes.Encrypted | FileAttributes.System | FileAttributes.Hidden);
                }

                if (!file.FullName.Contains("CustomBlackList") && !file.FullName.Contains("SavedFilteringSettings") && !file.FullName.Contains("SavedScheduel"))
                {
                    fileStreams.Add(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
            }
            catch
            {
            }
        }

        public static void StopCatchingSystemFiles()
        {
            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MMB");
            string exePath = AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo dataDirectory = new DirectoryInfo(dataPath);
            foreach (FileInfo file in dataDirectory.GetFiles())
            {
                try
                {
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                }
                catch
                {}
            }

            DirectoryInfo exeFolder = new DirectoryInfo(exePath);
            foreach (FileInfo file in exeFolder.GetFiles())
            {
                try
                {
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                }
                catch
                { }
            }

            foreach (FileStream stream in fileStreams)
            {
                try
                {
                    stream.Close();
                    stream.Dispose();
                    fileStreams.Remove(stream);
                }
                catch
                { }
            }
            fileStreams.Clear();
            fileStreams = null;
        }
    }
}
