using F4E_GUI;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for EnterAdminPasswordWindow.xaml
    /// </summary>
    public partial class EnterAdminPasswordWindow : Window
    {
        public enum askingMode
        {
            login,uninstall
        }

        public EnterAdminPasswordWindow(Window OwnerWindow, askingMode mode)
        {
            InitializeComponent();
            this.Owner = OwnerWindow;
            passwordTB.Focus();
            SetGUIbyMode(mode);
        }

        private void SetGUIbyMode(askingMode mode)
        {
            switch(mode)
            {
                case askingMode.login:
                    welcomeLabel.Text = "ברוך הבא, " + FilteringSystem.GetCurrentFilteringSettings().GetAdminName() + "!";
                    break;
                case askingMode.uninstall:
                    welcomeLabel.Text = "תהליך הסרת התקנה";
                    break;
            }
        }

        int attemps = 3;
        public Boolean correctPassword = false;
        Boolean closeButtonWasClicked = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FilteringSystem.LoginWithAdminPassword(passwordTB.Password))
            {
                this.DialogResult = true;
                correctPassword = true;
                this.Close();
                InternetBlocker.Block(false);
            }
            else
            {
                attemps--;
                if (attemps > 0)
                    CustomMessageBox.ShowDialog(this, "לצערנו, הקלדת את הסיסמה הלא נכונה. נותרו לך עוד " + attemps + " ניסיונות. לאחר מכן הגלישה ברשת תיחסם, ותאופשר בהכנסת סיסמה בלבד", "סיסמה שגויה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                else
                {
                    CustomMessageBox.ShowDialog(this, "האינטרנט במחשב זה נחסם עקב שימוש בסיסמה שגויה", "האינטרנט נחסם", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                    MailsSender.SendWrongPasswordAlert();
                    InternetBlocker.Block(true);
                }

            }
            passwordTB.Password = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!correctPassword && !closeButtonWasClicked)
            {
                e.Cancel = true;
            }
            closeButtonWasClicked = false;
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            closeButtonWasClicked = true;
            DialogResult = false;
            this.Close();
            this.Owner.Close();
        }

        private void passwordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            submitButton.IsEnabled = passwordTB.Password.Length > 0 ? true : false;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(CustomMessageBox.ShowDialog(null,"האם ברצונך לשלוח תזכורת סיסמה לכתובת המייל שהוגדרה במערכת?","לשלוח תזכורת?",CustomMessageBox.CustomMessageBoxTypes.Question,"כן","לא")==true)
            {
                MailsSender.SendPasswordReminderMail();
            }
        }
    }
}
