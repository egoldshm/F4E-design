using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F4E___Uninstaller
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\F4E by MMB.exe");
        }

        protected override void OnBeforeUninstall (IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            if(Process.GetProcessesByName("F4E by MMB.exe").Length>0)
            {
                throw new InstallException("לא ניתן להסיר התקנה כאשר מופע של התוכנה פעיל. יש לסגור את כל המופעים של התוכנה ולאחר מכן לנסות שנית.");
            }

            EnterPasswordForm enterPasswordForm = new EnterPasswordForm(true);
            enterPasswordForm.ShowDialog();

            if (enterPasswordForm.IsCorrectPassword())
            {
                if (enterPasswordForm.IsSuccessfullyUninstalled())
                {
                    Tools.RunCmdCommand("sc delete guiadapter");
                }
                else
                {
                    throw new InstallException("Error during uninstall process...");
                }
            }
            else
            {
                throw new InstallException("Administrator password is incorrect, message has been sent to the administrator.");
            }
        }
    }
}
