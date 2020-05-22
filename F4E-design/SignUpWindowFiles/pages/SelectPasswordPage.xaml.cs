using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F4E_design.SignUpWindowFiles.pages
{
    /// <summary>
    /// Interaction logic for SelectPassword.xaml
    /// </summary>
    public partial class SelectPasswordPage : Page
    {
        public string enteredPassword;

        private SelectPasswordPage()
        {
            InitializeComponent();
        }
        //singelton
        public SignUpWindow Window { get; set; }
        private static SelectPasswordPage instance = null;
        public static SelectPasswordPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectPasswordPage();
                return instance;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeIn") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }

        private void PasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordTB.Password.All(Char.IsLetterOrDigit) && PasswordTB.Password.Length >= 4)
            {
                PasswordTB.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 33, 150, 243));
                if (PasswordTB.Password.Equals(ConfirmPasswordTB.Password))
                {
                    enteredPassword = PasswordTB.Password;
                    ConfirmPasswordTB.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    Window.SetNextEnabled(true);
                }
                else
                {
                    ConfirmPasswordTB.Foreground = new SolidColorBrush(Colors.Red);
                    Window.SetNextEnabled(false);
                }
            }
            else
            {
                Window.SetNextEnabled(false);
                PasswordTB.Foreground = new SolidColorBrush(Colors.Red);
                if(ConfirmPasswordTB.Password.Length>0)
                    ConfirmPasswordTB.Foreground = new SolidColorBrush(Colors.Red);
                else
                    ConfirmPasswordTB.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 33, 150, 243));
            }
        }
    }
}
