using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace F4E___Uninstaller
{
    class ServiceAdapter
    {
        public static Boolean UninstallService(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", exePath });
            }
            catch
            { 
            }
            return true;
        }
    }
}
