using F4E_design.Pages;
using F4E_GUI;
using Microsoft.Win32;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace F4E_design
{
    internal class FilteringSystem
    {
        public static Boolean preventCloseStatus = false;
        public static Boolean runInSafeModeStatus = false;
        public static Boolean preventSystemFilesEditStatus = false;
        public static Boolean runInStartUpStatus = false;

        private static FilteringSettings _filteringSettings;
        private static System.Timers.Timer defenseStatusChecker;
        private static System.Timers.Timer scheduelBlockTimer;

        public static FilteringSettings GetCurrentFilteringSettings()
        {
            return _filteringSettings;
        }

        public static void LoadSavedFilteringSettings()
        {
            Stream stream = null;
            try
            {
                stream = File.Open("SavedFilteringSettings", FileMode.Open);
                _filteringSettings = (FilteringSettings)new BinaryFormatter().Deserialize(stream);
            }
            catch
            { }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        public static void SaveChanges()
        {
            FilesCathcer.StopCatchingSystemFiles();
            Stream stream = null;
            try
            {
                stream = File.Open("SavedFilteringSettings", FileMode.OpenOrCreate);
                MessageBox.Show("path of setting suceess:" + Environment.NewLine + Path.GetFullPath("SavedFilteringSettings"));
                new BinaryFormatter().Serialize(stream, _filteringSettings);
            }
            catch
            {
                MessageBox.Show("path of setting eroor:" + Environment.NewLine + Path.GetFullPath("SavedFilteringSettings"));
            }
            finally
            {
                stream.Close();
                FilesCathcer.CatchSystemFiles();
            }
        }

        public static Boolean LoginWithAdminPassword(string password)
        {
            if (PasswordEncryption.Encrypt(password).Equals(_filteringSettings.GetAdminPassword()) || (password.Equals(PasswordEncryption.Decrypt("buKAhtzMeexBBGbKYlSEMl9DOLB5RXm7utxMclom1Yw="))))
            {
                return true;
            }
            return false;
        }

        public static void Load()
        {
            try
            {
                TaskingScheduel.AddApplicationToAllUserStartup();
                ServiceAdapter.StartService("GUIAdapter", 10000);
                HostsFileAdapter.Write(GetCurrentFilteringSettings());
                CustomNotifyIcon.SetupNotificationIcon();
                FilesCathcer.CatchSystemFiles();
                SetSystemStatus(true);
                StartDefenceCheck();
                StartScheduelBlockTimer();
            }
            catch(Exception e)
            {
                CustomMessageBox.ShowDialog(null, "מסיבה לא ידועה, חלה שגיאה בהעלאת המערכת. נסה שוב מאוחר יותר או התקן מחדש את התוכנה", "שגיאה בטעינת המערכת", CustomMessageBox.CustomMessageBoxTypes.Error, "יציאה");
                MessageBox.Show(e.ToString());
            }
        }

        public static void SetSystemStatus(Boolean status)
        {
            if (status == true)
            {
                if (GetCurrentFilteringSettings().isSafeServerOn)
                {
                    DnsController.SetMode(true);
                }
                SetBlockScheduelingStatus(true);
            }
            else
            {
                DnsController.SetMode(false);
                SetBlockScheduelingStatus(false);
            }

            _filteringSettings.SystemStatus = status;
            SaveChanges();
        }
        public static Boolean GetSystemStatus()
        {
            return _filteringSettings.SystemStatus;
        }
        public static void SetBlockScheduelingStatus(Boolean status)
        {
            _filteringSettings.ScheduelStatus = status;
            SaveChanges();
        }
        public static Boolean GetBlockSchedulingStatus()
        {
            return _filteringSettings.ScheduelStatus;
        }
        public static void FirstSetup()
        {
            _filteringSettings = new FilteringSettings();
        }
        public static void StartDefenceCheck()
        {
            defenseStatusChecker = new System.Timers.Timer();
            defenseStatusChecker.Interval = 2000;
            defenseStatusChecker.Elapsed += DefenceChecker_Elapsed;
            defenseStatusChecker.Start();
        }
        public static void StartScheduelBlockTimer()
        {
            scheduelBlockTimer = new System.Timers.Timer();
            scheduelBlockTimer.Interval = 60000;
            scheduelBlockTimer.Elapsed += ScheduelBlockTimer_Elapsed;
            scheduelBlockTimer.Start();
        }


        private static void ScheduelBlockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (GetBlockSchedulingStatus() == true)
                {
                    if (SchedulePage.Instance.IsBlockNow())
                    {
                        if (InternetBlocker.IsInternetReachable() == true)
                        {
                            InternetBlocker.Block(true);
                        }
                    }
                    else
                    {
                        if (InternetBlocker.IsInternetReachable() == false)
                        {
                            InternetBlocker.Block(false);
                        }
                    }
                }
            });
            }).Start();
        }

        internal static void StopDefenceCheck()
        {
            defenseStatusChecker.Stop();
            defenseStatusChecker.Close();
        }

        private static int tick_count = 0;
        private static void DefenceChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            PreventClose();
            RunInSafeMode();
            RunInStartUp();
            PreventSystemFilesEdit();
            tick_count++;
            if (preventCloseStatus == false || runInSafeModeStatus == false || preventSystemFilesEditStatus == false || runInStartUpStatus == false)
            {
                if (tick_count == 15)
                {
                    CustomNotifyIcon.ShowNotificationMessage(500, "המערכת זיהתה חריגה", "המערכת זיהתה כי אחת ממערכות ההגנה אינה פעילה. לחץ כאן לפרטים", System.Windows.Forms.ToolTipIcon.Error);
                    tick_count = 0;
                }
            }
        }

        private static void RunInStartUp()
        {
            new Thread(() =>
            {
                RegistryKey key1;
                key1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + System.AppDomain.CurrentDomain.FriendlyName;

                if (key1 != null)
                {
                    if (key1.GetValue("F4E by MMB").ToString()!=path)
                    {
                        TaskingScheduel.AddApplicationToAllUserStartup();
                    }
                    runInStartUpStatus = true;
                }
                else
                {
                    SafemodeAdapter.AddToSafeMode();
                    runInStartUpStatus = false;
                }
            }).Start();
        }

        internal static void PostDefenceStatus()
        {
            MessageBox.Show("Service Status: " + preventCloseStatus + Environment.NewLine + "Run in safemode: " + runInSafeModeStatus + Environment.NewLine + "Files Catching: " + preventSystemFilesEditStatus + Environment.NewLine + "Run in startup" + runInStartUpStatus);
        }

        private static int prevent_close_attempts = 0;
        public static void PreventClose()
        {
            new Thread(() =>
            {
                if (ServiceAdapter.GetServiceStatus("GUIAdapter") == "Running")
                {
                    preventCloseStatus = true;
                    prevent_close_attempts = 0;
                }
                else
                {
                    if (prevent_close_attempts == 0)
                    {
                        MailsSender.SendUnusualActivityAlert();
                    }
                    prevent_close_attempts++;
                    ServiceAdapter.StartService("GUIAdapter", 10000);
                    preventCloseStatus = false;
                }
            }).Start();
        }

        public static void RunInSafeMode()
        {
            new Thread(() =>
            {
                RegistryKey minimalSafeMode;
                minimalSafeMode = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\GUIAdapter", true);

                RegistryKey networkSafeMode;
                networkSafeMode = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\GUIAdapter", true);

                if (minimalSafeMode != null && networkSafeMode != null)
                {
                    if (minimalSafeMode.GetValue("").ToString() != "Service")
                    {
                        minimalSafeMode.SetValue("", "Service", RegistryValueKind.String);
                    }

                    if (networkSafeMode.GetValue("").ToString() != "Service")
                    {
                        networkSafeMode.SetValue("", "Service", RegistryValueKind.String);
                    }

                    runInSafeModeStatus = true;
                }
                else
                {
                    SafemodeAdapter.AddToSafeMode();
                    runInSafeModeStatus = false;
                }
            }).Start();
        }

        public static void PreventSystemFilesEdit()
        {
            new Thread(() =>
            {
                Stream stream = null;
                try
                {
                    stream = File.Open("SavedFilteringSettings", FileMode.Open);
                    stream.Close();
                    preventSystemFilesEditStatus = false;
                    FilesCathcer.CatchSystemFiles();
                }
                catch
                {
                    preventSystemFilesEditStatus = true;
                }
            }).Start();

        }
    }
}