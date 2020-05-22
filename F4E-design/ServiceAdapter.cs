using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Text;
using System.Threading.Tasks;

namespace F4E_design
{
    class ServiceAdapter
    {
        private static readonly string SERVICE_NAME = "GUIAdapter";

        public enum CustomCommends { startScheduelBlocking = 128, releaseScheduelBlocking = 129 }

        public static void InstallService(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { exePath });
            }
            catch
            {
                CustomMessageBox.ShowDialog(null, "שגיאה בהתקנת תהליך" , "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        public static void UninstallService(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", exePath });
            }
            catch
            {
                CustomMessageBox.ShowDialog(null, "שגיאה בהסרת תהליך", "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            { }
        }

        public static Boolean StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CustomCommend(string serviceName, int commend)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, Environment.MachineName, serviceName);//this will grant permission to access the Service
                scp.Assert();
                sc.Refresh();
                sc.ExecuteCommand(commend);
            }
            catch
            {
            }
        }

        public static void StartInternetBlocking()
        {
            CustomCommend(SERVICE_NAME, (int)CustomCommends.startScheduelBlocking);
        }

        public static void StopInterntBlocking()
        {
            CustomCommend(SERVICE_NAME, (int)CustomCommends.releaseScheduelBlocking);
        }

        public static string GetServiceStatus(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                return service.Status.ToString();
            }
            catch
            {
                return "Not Found";
            }
        }
    }
}
