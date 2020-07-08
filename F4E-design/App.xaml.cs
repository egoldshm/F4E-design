using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
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

        public static void NotifyForNewVersion(Window window)
        {
           if(CheckForUpdates()==true)
            {
                if (CustomMessageBox.ShowDialog(window, "גרסה חדשה זמינה להורדה עכשיו באתר. האם ברצונך לעבור לאתר כעת?", "גרסה חדשה זמינה", CustomMessageBox.CustomMessageBoxTypes.Question, "עבור לאתר", "יותר מאוחר")==true)
                {
                    Process.Start("https://f4e.mmb.org.il");
                }
            }
        }
        public static void NotifyForVersionOutOfUse(Window window)
        {
            if (IsActiveVersion() == false)
            {
                CustomMessageBox.ShowDialog(window, "נראה כי גרסה זו הינה גרסה ישנה של האפליקציה וכי היא יצאה מכלל שימוש. יש לעדכן את התוכנה דרך האתר ולאחר מכן לנסות שנית.", "גרסה זו מחוץ לשימוש", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                App.FullExit();
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

        public static Boolean CheckForUpdates()
        {
            string last_version = Tools.GetTextFromUri("http://f4e.mmb.org.il/data/last_version");
            string current_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (last_version != null)
            {
                return !(last_version == current_version);
            }
            else
            {
                return false;
            }
        }

        public static Boolean IsActiveVersion()
        {
            string active_versions = Tools.GetTextFromUri("http://f4e.mmb.org.il/data/active_versions");
            string current_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (active_versions != null)
            {
                return (active_versions.Contains(current_version));
            }
            else
            {
                return false;
            }
        }

        public static void FullExit()
        {
            FilteringSystem.StopDefenceCheck();
            InternetBlocker.Block(false);
            CustomNotifyIcon.Hide();
            ServiceAdapter.CustomCommend("GUIAdapter", (int)ServiceAdapter.CustomCommends.kill);
            ServiceAdapter.StopService("GUIAdapter", 10000);
            Environment.Exit(Environment.ExitCode);
        }
    }
}
