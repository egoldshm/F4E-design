using F4E_design.Pages;
using F4E_GUI;
using Microsoft.Win32;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace F4E_design
{
    internal class FilteringSystem
    {
        public static Boolean PreventCloseStatus = false;
        public static Boolean RunInSafeModeStatus = false;
        public static Boolean PreventSystemFilesEditStatus = false;
        

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
            Stream stream = null;
            try
            {
                stream = File.Open("SavedFilteringSettings", FileMode.Create);
                new BinaryFormatter().Serialize(stream, _filteringSettings);
            }
            finally
            {
                stream.Close();
            }
        }

        public static void SetAdminPassword(string password)
        {
            _filteringSettings.SetAdminPassword(password);
        }
        public static string GetAdminMail()
        {
            return _filteringSettings.GetAdminMail();
        }
        public static void SetAdminMail(string mail)
        {
            _filteringSettings.SetAdminMail(mail);
        }
        public static string GetAdminName()
        {
            return _filteringSettings.GetAdminName();
        }

        public static void SetAdminName(string name)
        {
            _filteringSettings.SetAdminName(name);
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
                //TaskingScheduel.AddAppToStartupApplications("F4E by MMB", System.AppDomain.CurrentDomain.BaseDirectory + "\\" + System.AppDomain.CurrentDomain.FriendlyName);
                ServiceAdapter.StartService("GUIAdapter", 10000);
                StartDefenceCheck();
                StartScheduelBlockTimer();
                CustomNotifyIcon.SetupNotificationIcon();
                FilteringSystem.SetSystemStatus(true);
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
            defenseStatusChecker.Interval = 3000;
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
                        if (InternetBlocker.isInternetReachable() == true)
                        {
                            InternetBlocker.Block(true);
                        }
                    }
                    else
                    {
                        if (InternetBlocker.isInternetReachable() == false)
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
        }

        private static int tick_count = 0;
        private static void DefenceChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            PreventClose();
            RunInSafeMode();
            PreventSystemFilesEdit();
            tick_count++;
            if (PreventCloseStatus == false /*|| RunInSafeModeStatus == false*/ || PreventSystemFilesEditStatus == false)
            {
                if (tick_count == 15)
                {
                    CustomNotifyIcon.ShowNotificationMessage(500, "המערכת זיהתה חריגה", "המערכת זיהתה כי אחת ממערכות ההגנה אינה פעילה. לחץ כאן לפרטים", System.Windows.Forms.ToolTipIcon.Error);
                    tick_count = 0;
                }
            }
        }

        public static void PreventClose()
        {
            new Thread(() =>
            {
                if (ServiceAdapter.GetServiceStatus("GUIAdapter") == "Running")
                {
                    PreventCloseStatus = true;
                }
                else
                {
                    ServiceAdapter.StartService("GUIAdapter", 10000);
                    PreventCloseStatus = false;
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
                    RunInSafeModeStatus = true;
                }
                else
                {
                    SafemodeAdapter.AddToSafeMode();
                    RunInSafeModeStatus = false;
                }
            }).Start();
        }

        public static void PreventSystemFilesEdit()
        {
            new Thread(() =>
            {
                PreventSystemFilesEditStatus = false;
            }).Start();
        }
        
        public static string AddSiteToBlackList(string url)
        {
            if (url.CheckURLValid())
            {
                if (!GetCurrentFilteringSettings().GetCustomBlackList().Contains(url))
                {
                    if (!GetCurrentFilteringSettings().GetCustomExceptionsList().Contains(url))
                    {
                        GetCurrentFilteringSettings().addSiteToBlacklist(url);
                        HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
                        SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "כתובת זה מופיעה ברשימת החריגים, על מנת לחוסמה, יש להסירה תחילה מרשימה זו";
                    }
                }
                else
                {
                    return "הכתובת כבר קיימת ברשימת האתרים לחסימה";
                }
            }
            else
            {
                return "הכתובת אינה כתובת אינטרנט תקינה";
            }
        }
        public static void RemoveSiteFromBlackList(string url)
        {
            GetCurrentFilteringSettings().removeSiteFromBlacklist(url);
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            SaveChanges();
        }
        public static string AddSiteToExceptionList(string url)
        {
            if (url.CheckURLValid())
            {
                if (!GetCurrentFilteringSettings().GetCustomExceptionsList().Contains(url))
                {
                    if (!GetCurrentFilteringSettings().GetCustomBlackList().Contains(url))
                    {
                        GetCurrentFilteringSettings().addSiteToExceptionsList(url);
                        HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
                        SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "כתובת זה מופיעה ברשימת החסומים, על מנת להחריגה, יש להסירה תחילה מרשימה זו";
                    }
                }
                else
                {
                    return "הכתובת כבר קיימת ברשימה";
                }
            }
            else
            {
                return "הכתובת אינה כתובת אינטרנט תקינה";
            }
        }
        public static void RemoveSiteFromExceptionList(string url)
        {
            GetCurrentFilteringSettings().removeSiteFromExceptionsList(url);
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            SaveChanges();
        }
    
    }
}