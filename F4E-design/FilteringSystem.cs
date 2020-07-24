using F4E_design.Pages;
using F4E_GUI;
using IWshRuntimeLibrary;
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
        public static Boolean isServiceIsOn = false;
        public static Boolean runInSafeModeStatus = false;
        public static Boolean preventSystemFilesEditStatus = false;
        public static Boolean runInStartUpStatus = false;
        public static Boolean dataCatched = false, exeCatched = false;
        private static Boolean serviceHasDetected = false;


        private static FilteringSettings _filteringSettings;
        private static System.Timers.Timer defenseStatusChecker;
        private static Stream savedSettingsStream;


        public static FilteringSettings GetCurrentFilteringSettings()
        {
            return _filteringSettings;
        }

        public static void LoadSavedFilteringSettings()
        {
            try
            {
                savedSettingsStream = System.IO.File.Open(Path.Combine(App.GetAppDataFolder(), "SavedFilteringSettings"), FileMode.OpenOrCreate);
                _filteringSettings = (FilteringSettings)new BinaryFormatter().Deserialize(savedSettingsStream);
            }
            catch
            { }
        }
        public static void SaveChanges()
        {
            try
            {
                savedSettingsStream.Close();
                System.IO.File.Delete(Path.Combine(App.GetAppDataFolder(), "SavedFilteringSettings"));
                savedSettingsStream = System.IO.File.Open(Path.Combine(App.GetAppDataFolder(), "SavedFilteringSettings"), FileMode.OpenOrCreate);
                new BinaryFormatter().Serialize(savedSettingsStream, _filteringSettings);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"SaveChanges Error");
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
            HostsFileAdapter.Write(GetCurrentFilteringSettings());
            CustomNotifyIcon.SetupNotificationIcon();
            ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends._continue);
            ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.startCatchFiles);
            SetSystemStatus(true);
            StartDefenceCheck();
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
        public static void UpdateAllSettings()
        {
            HostsFileAdapter.Write(GetCurrentFilteringSettings());
            SchedulePage.Instance.UpdateBlockingMode();
            ProblematicAppsBlocker.UpdateBlockedList();
        }
        
        public static void StartDefenceCheck()
        {
            defenseStatusChecker = new System.Timers.Timer();
            defenseStatusChecker.Interval = 1000;
            defenseStatusChecker.Elapsed += DefenceChecker_Elapsed;
            defenseStatusChecker.Start();
        }
     
        internal static void StopDefenceCheck()
        {
            defenseStatusChecker.Stop();
            defenseStatusChecker.Close();
        }

        private static int tick_count = 0;
        private static int scheduelUpdate = 0;
        private static void DefenceChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            KeepServiceOn();
            RunInSafeMode();
            RunInStartUp();
            PreventSystemFilesEdit();
            ProblematicAppsBlocker.Block();
            tick_count++;
            if (isServiceIsOn == true)
            {
                if (runInSafeModeStatus == false || preventSystemFilesEditStatus == false || runInStartUpStatus == false)
                {
                    if (!runInSafeModeStatus)
                        RunInSafeMode();

                    if (!preventSystemFilesEditStatus)
                        PreventSystemFilesEdit();

                    if (!runInStartUpStatus)
                        RunInStartUp();

                    if (tick_count > 1200)
                    {
                        CustomNotifyIcon.ShowNotificationMessage(500, "המערכת זיהתה חריגה", "המערכת זיהתה כי אחת ממערכות ההגנה אינה פעילה. לחץ כאן לפרטים", System.Windows.Forms.ToolTipIcon.Error);
                        tick_count = 0;
                    }
                }
            }
            else
            {
                if (tick_count > 1200)
                {
                    CustomNotifyIcon.ShowNotificationMessage(500, "הסינון אינו יציב", "אחת מהמערכות הקריטיות לפעילות הסינון אינה פעילה, הודעה נשלחה למנהל המערכת.", System.Windows.Forms.ToolTipIcon.Error);
                    tick_count = 0;
                }
            }

            scheduelUpdate++;
            if(scheduelUpdate==60)
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
        }
        public static void RunInStartUp()
        {
            new Thread(() =>
            {
                try
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\StartUp\‏‏F4E by MMB.lnk";
                    if (System.IO.File.Exists(path))
                    {
                        // WshShellClass shell = new WshShellClass();
                        WshShell shell = new WshShell(); //Create a new WshShell Interface
                        IWshShortcut link = (IWshShortcut)shell.CreateShortcut(path); //Link the interface to our shortcut

                        if (link.TargetPath == Assembly.GetExecutingAssembly().Location)
                        {
                            runInStartUpStatus = true;
                        }
                        else
                        {       
                            runInStartUpStatus = false;
                            ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.addToStartUp);
                        }
                    }
                    else
                    {
                        runInStartUpStatus = false;
                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.addToStartUp);
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }).Start();
        }

        internal static void PostDefenceStatus()
        {
            MessageBox.Show("Service Status: " + isServiceIsOn + Environment.NewLine + "Run in safemode: " + runInSafeModeStatus + Environment.NewLine + "Files Catching: " + preventSystemFilesEditStatus + Environment.NewLine + "Run in Startup: " + runInStartUpStatus+ " |dataCatched: " + dataCatched+ " |exeCatched: " + exeCatched);
        }

        private static int prevent_close_attempts = 0;
        public static void KeepServiceOn()
        {
            new Thread(() =>
            {
                if (ServiceAdapter.GetServiceStatus("GUIAdapter") == "Running")
                {
                    isServiceIsOn = true;
                    serviceHasDetected = true;
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

                    if(InternetBlocker.IsInternetReachable())
                        InternetBlocker.Block(true);

                    if (ServiceAdapter.GetServiceStatus("GUIAdapter") == "Stopped")
                    {
                        if (serviceHasDetected)
                            BootController.DoExitWin(BootController.EWX_REBOOT);
                    }

                    isServiceIsOn = false;
                }
            }).Start();
        }

        public static void RunInSafeMode()
        {
            new Thread(() =>
            {
                runInSafeModeStatus = SafeModeAdapter.IsRunInSafeMode();
                if (!runInSafeModeStatus)
                {
                    ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.addToSafeMode);
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
                    string path = Path.Combine(App.GetAppDataFolder(), "SavedFilteringSettings");
                    stream = System.IO.File.Open(path, FileMode.Open);
                    stream.Close();
                    dataCatched = false;
                    ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.startCatchFiles);
                }
                catch
                {
                    dataCatched = true;
                }

                try
                {
                    string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "F4E by MMB.exe");
                    stream = System.IO.File.Open(path, FileMode.Open);
                    stream.Close();
                    exeCatched = false;
                    ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.startCatchFiles);
                }
                catch(Exception e)
                {
                    exeCatched = true;
                }

                preventSystemFilesEditStatus = (dataCatched && exeCatched);
            }).Start();

        }
    }
}