using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace F4E___Uninstaller
{
    class FilesCathcer
    {
        private static List<FileStream> fileStreams = new List<FileStream>();
        public static void CatchSystemFiles(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                if (!file.FullName.Contains("SavedFilteringSettings"))
                {
                    File.SetAttributes(file.FullName, FileAttributes.ReadOnly | FileAttributes.Encrypted | FileAttributes.System | FileAttributes.Hidden);
                }
                try
                {
                    //streamReaders.Add(new StreamReader(file.FullName));
                    fileStreams.Add(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
                catch
                { }
            }
        }
        public static void StopCatchingSystemFiles(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
            }
            foreach (FileStream stream in fileStreams)
            {
                stream.Close();
            }
        }
    }
}
