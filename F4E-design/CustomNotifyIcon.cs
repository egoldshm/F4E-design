using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F4E_design
{
    class CustomNotifyIcon
    {
        [DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        private static NotifyIcon _notifyIcon = new NotifyIcon();

        public static void SetupNotificationIcon()
        {
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            _notifyIcon.Visible = true;
            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.BalloonTipClicked += NotifyIcon_Click;
        }
        private static void NotifyIcon_Click(object sender, EventArgs e)
        {
            MainWindow window = App.Current.MainWindow as MainWindow;
            if (window == null)
            {
                window = new MainWindow();
            }
            if (window.IsVisible == false)
            {
                try
                {
                    window.ShowDialog();
                }
                catch
                {
                    window.Close();
                    window = new MainWindow();
                    window.ShowDialog();
                }
            }
            SwitchToThisWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, true);
        }

        public static void ShowNotificationMessage(int timeout, string title, string text, System.Windows.Forms.ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(timeout, title, text, icon);
        }

        internal static void Hide()
        {
            _notifyIcon.Visible = false;
        }
    }
}
