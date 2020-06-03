using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (NoOtherProcessOpen())
            {
                if (HasAdministratorPrivilege())
                {
                    FilteringSystem.LoadSavedFilteringSettings();
                    if (IsFirstOpening())
                    {
                        new SignUpWindowFiles.SignUpWindow().Show();     
                    }
                    else
                    {
                        FilteringSystem.Load();
                        new MainWindow().Show();
                    }
                }
                else
                {
                    if (StartAgainAsAdmin())
                    {
                        //By opening a new administrator-privileged process, close the current process.
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        CustomMessageBox.ShowDialog(null, "אפליקציה זו חייבת לרוץ כמנהל, האינטרנט מושבת.", "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                        Application.Current.Shutdown();
                    }
                }
            }
            else
            {
                if (Environment.GetCommandLineArgs().Length > 1)
                    if (Environment.GetCommandLineArgs()[0] != "runAgainAsAdmin")
                        Application.Current.Shutdown();
            }
        }

        private bool IsFirstOpening()
        {
           // return false;
           return (FilteringSystem.GetCurrentFilteringSettings()==null);
        }

        public static bool NoOtherProcessOpen()
        {
            return (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() == 1);
        }

        public static Boolean HasAdministratorPrivilege()
        {
            //return whether the current instance of the system has an administrator privilege
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static Boolean StartAgainAsAdmin()
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
            processInfo.Arguments = "runAgainAsAdmin";
            try
            {
                System.Diagnostics.Process.Start(processInfo);
                return true;
            }
            catch (Exception)
            {
                // The user did not allow the application to run as administrator
                return false;
            }
        }
     
    }
}
