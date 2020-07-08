using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;

namespace F4E_design
{
    class ServiceAdapter
    {
        private static readonly string SERVICE_NAME = "GUIAdapter";

        public enum CustomCommends { startScheduelBlocking = 128, releaseScheduelBlocking = 129 , kill = 131 }

        public static void InstallService(string exePath)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { exePath });
            }
            catch
            { }
        }

        public static void UninstallService()
        {
            try
            {
                string servicePath = Assembly.GetExecutingAssembly().CodeBase;
                servicePath = servicePath.Replace("F4E by MMB.exe", "F4E-Service.exe");
                ServiceAdapter.CustomCommend("GUIAdapter", (int)CustomCommends.kill);
                StopService("GUIAdapter", 10000);
                ManagedInstallerClass.InstallHelper(new string[] { "/u", servicePath});
            }
            catch
            {
                CustomMessageBox.ShowDialog(null, "שגיאה בהסרת תהליך", "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        private static int runAttemps = 0;
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
            {
                string servicePath = Assembly.GetExecutingAssembly().CodeBase;
                servicePath = servicePath.Replace("F4E by MMB.exe", "F4E-Service.exe");
                ServiceAdapter.InstallService(servicePath);
                if (GetServiceStatus(serviceName) != "Running")
                {
                    if (runAttemps < 6)
                    {
                        runAttemps++;
                        StartService(serviceName, 10000);
                        System.Threading.Thread.Sleep(400);
                    }
                    else
                    {
                        //CustomMessageBox.ShowDialog(null, e.Message, "הפעלת שירות נכשלה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                        runAttemps = 0;
                    }
                }
                else
                {
                    runAttemps = 0;
                }
            }
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