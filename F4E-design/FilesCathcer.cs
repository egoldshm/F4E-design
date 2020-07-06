using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace F4E_GUI
{
    class FilesCathcer
    {
        private static List<FileStream> fileStreams = new List<FileStream>();
        public static void CatchSystemFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
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
        public static void StopCatchingSystemFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            foreach (FileInfo file in directory.GetFiles())
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
            }
            foreach (FileStream stream in fileStreams)
            {
                stream.Close();
            }
        }
        private static void SetDirectoryOnlySystemAccess(string path)
        {
            try
            {
                SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
                DirectorySecurity securityRules = new DirectorySecurity();
                FileSystemAccessRule fsRule = new FileSystemAccessRule(sid, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);

                var existingACL = Directory.GetAccessControl(path);
                foreach (FileSystemAccessRule rule in existingACL.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    existingACL.RemoveAccessRuleAll(rule);
                }

                existingACL.AddAccessRule(fsRule);
                Directory.SetAccessControl(path, existingACL);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);   
            }
        }
    }
}
