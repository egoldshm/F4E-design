using F4E_design.Pages;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Windows;

namespace F4E_design
{
    internal class FilteringSystem
    {
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
                stream = File.Open("SavedProfile", FileMode.Open);
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
                stream = File.Open("SavedProfile", FileMode.Create);
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
            TaskingScheduel.AddAppToStartupApplications("F4E by MMB", System.AppDomain.CurrentDomain.BaseDirectory + "\\" + System.AppDomain.CurrentDomain.FriendlyName);
            ServiceAdapter.StartService("GUIAdapter", 10000);
            StartDefenceCheck();
            StartScheduelBlockTimer();
            CustomNotifyIcon.SetupNotificationIcon();
            FilteringSystem.SetSystemStatus(true);
        }

        public static void SetSystemStatus(Boolean status)
        {
            if (status == true)
            {
                if (GetCurrentFilteringSettings().isSafeServerOn)
                {
                    DnsController.setMode(true);
                }
                SetBlockScheduelingStatus(true);
            }
            else
            {
                DnsController.setMode(false);
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
            defenseStatusChecker.Interval = 10000;
            defenseStatusChecker.Elapsed += DefenceChecker_Elapsed;
            defenseStatusChecker.Start();
        }
        public static void StartScheduelBlockTimer()
        {
            scheduelBlockTimer = new System.Timers.Timer();
            scheduelBlockTimer.Interval = 10000;
            scheduelBlockTimer.Elapsed += ScheduelBlockTimer_Elapsed;
            scheduelBlockTimer.Start();
        }

        private static void ScheduelBlockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (GetBlockSchedulingStatus() == true)
                {
                    if (SchedulePage.Instance.isBlockNow())
                    {
                        if (InternetBlocker.isInternetReachable() == true)
                        {
                            InternetBlocker.Block(true);
                            ServiceAdapter.StartInternetBlocking();
                            return;
                        }
                    }
                }
                if (InternetBlocker.isInternetReachable() == false)
                {
                    ServiceAdapter.StopInterntBlocking();
                    InternetBlocker.Block(false);
                }
            });
        }

        private static int tick_count = 0;
        private static void DefenceChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            tick_count++;
            if (GetClosingPreventStatus() == false || GetHostCatchStatus() == false)
            {
                ServiceAdapter.StartService("GUIAdapter", 10000);
                if (tick_count == 6)
                {
                    CustomNotifyIcon.ShowNotificationMessage(500, "המערכת זיהתה חריגה", "המערכת זיהתה כי אחת ממערכות ההגנה אינה פעילה. לחץ כאן לפרטים", System.Windows.Forms.ToolTipIcon.Error);
                    tick_count = 0;
                }
            }
        }

        public static Boolean GetHostCatchStatus()
        {
            return false;
        }

        public static Boolean GetClosingPreventStatus()
        {
            if (ServiceAdapter.GetServiceStatus("GUIAdapter") == "Running")
            {
                return true;
            }
            return false;
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
            SaveChanges();
        }
    }
}