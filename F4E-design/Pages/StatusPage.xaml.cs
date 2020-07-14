using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for StatusPage.xaml
    /// </summary>
    public partial class StatusPage : Page
    {
        System.Timers.Timer statusCheckerTimer;
        private StatusPage()
        {
            InitializeComponent();
            statusCheckerTimer = new System.Timers.Timer();
            statusCheckerTimer.Interval = 3000;
            statusCheckerTimer.Elapsed += StatusCheckerTimer_Elapsed;
            statusCheckerTimer.Start();
        }

        //singelton
        private static StatusPage instance = null;
        public static StatusPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new StatusPage();
                return instance;
            }
        }

        private void StatusCheckerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateStatusGUI();
        }
        private void UpdateStatusGUI()
        {
            UpdateSystemStatus();
            UpdateSafeServerStatus();
            UpdateScheduelingSystemStatus();
            UpdateClosingPreventStatus();
            UpdateHostCatchingStatus();
        }
        private void UpdateHostCatchingStatus()
        {
            FilteringSystem.PreventSystemFilesEdit();
            Dispatcher.Invoke(new Action(() =>
            {
                if (FilteringSystem.preventSystemFilesEditStatus == true)
                {
                    hostCatchStatusLabel.Content = "פעיל";
                    hostCatchStatusLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 9, 163, 3));
                }
                else
                {
                    hostCatchStatusLabel.Content = "לא פעיל";
                    hostCatchStatusLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 252, 104, 78));
                }
            }));
        }
        private void UpdateClosingPreventStatus()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if ((FilteringSystem.isServiceIsOn && FilteringSystem.runInSafeModeStatus && FilteringSystem.runInStartUpStatus) == true)
                {
                    preventClosingStatusLabel.Content = "פעיל";
                    preventClosingStatusLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 9, 163, 3));
                }
                else
                {
                    preventClosingStatusLabel.Content = "לא פעיל";
                    preventClosingStatusLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 252, 104, 78));
                }
            }));
        }
        private void UpdateGUIStatus(string URI_ON_IMAGE, string URI_OFF_IMAGE, Boolean status, System.Windows.Controls.Image image)
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (status == true)
                    {
                        if (image.Name.EndsWith("Active"))
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(URI_ON_IMAGE, UriKind.Relative);
                            bitmapImage.CacheOption = BitmapCacheOption.None;
                            image.Name = image.Name.Replace("Active", "");
                            image.Source = bitmapImage;
                            bitmapImage.EndInit();
                        }
                    }
                    else
                    {
                        if (!image.Name.EndsWith("Active"))
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(URI_OFF_IMAGE, UriKind.Relative);
                            bitmapImage.CacheOption = BitmapCacheOption.None;
                            image.Name = image.Name != null ? image.Name + "Active" : "Active";
                            image.Source = bitmapImage;
                            bitmapImage.EndInit();
                        }
                    }
                }));
            }).Start();
        }
        private void UpdateSafeServerStatus()
        {
            new Thread(() =>
            {
                if ((DnsController.IsSafe(false) == false) && (FilteringSystem.GetCurrentFilteringSettings().isSafeServerOn) && (FilteringSystem.GetSystemStatus() == true))
                {
                    DnsController.SetMode(true);
                }
                UpdateGUIStatus("/images/statusPage/safeserver_on.png", "/images/statusPage/safeserver_off.png", DnsController.IsSafe(false), safeServerToggle);
            }).Start();
        }
        private void UpdateScheduelingSystemStatus()
        {
            new Thread(() =>
            {
                UpdateGUIStatus("/images/statusPage/scheduel_on.png", "/images/statusPage/scheduel_off.png", FilteringSystem.GetBlockSchedulingStatus(),scheduelToggle);
            }).Start();
        }
        private void UpdateSystemStatus()
        {
            new Thread(() =>
            {
                UpdateGUIStatus("/images/statusPage/status_on.png", "/images/statusPage/status_off.png", FilteringSystem.GetSystemStatus(), systemStatusToggle);
            }).Start();
        }
        
        private void sceduelToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FilteringSystem.SetBlockScheduelingStatus(!FilteringSystem.GetBlockSchedulingStatus());
        }

        private void systemStatusToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FilteringSystem.SetSystemStatus(!FilteringSystem.GetSystemStatus());
        }

        private void safeServerToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateGUIStatus("/images/statusPage/safeserver_on.png", "/images/statusPage/safeserver_off.png", DnsController.IsSafe(true), safeServerToggle);
        }

        private void preventClosingStatusLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (preventClosingStatusLabel.Content.ToString() == "לא פעיל")
            {
                FilteringSystem.PostDefenceStatus();
            }
        }

        private void hostCatchStatusLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (hostCatchStatusLabel.Content.ToString() == "לא פעיל")
            {
                FilteringSystem.PostDefenceStatus();
            }
        }
    }
}
