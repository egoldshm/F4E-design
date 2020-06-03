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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F4E_design.Pages
{
    /// <summary>
    /// Interaction logic for GeneralSettingsPage.xaml
    /// </summary>
    public partial class GeneralSettingsPage : Page
    {

        private GeneralSettingsPage()
        {
            InitializeComponent();
        }

        public void ResetGUI()
        {
            nameTB.Text = FilteringSystem.GetAdminName();
            passwordTB.Password = PasswordEncryption.Decrypt(FilteringSystem.GetCurrentFilteringSettings().GetAdminPassword());
            confirmPasswordTB.Password = passwordTB.Password;
            confirmPasswordTB.IsEnabled = false;
            confirmPasswordTB.Foreground = new SolidColorBrush(Colors.LimeGreen);
            mailTB.Text = FilteringSystem.GetAdminMail();
        }

        public MainWindow Window { get; set; }
        //singelton
        private static GeneralSettingsPage instance = null;
        public static GeneralSettingsPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new GeneralSettingsPage();
                return instance;
            }
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                if(passwordTB.Password.Length>3)
                {
                    passwordTB.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    passwordTB.Foreground = new SolidColorBrush(Colors.Red);
                }

                confirmPasswordTB.IsEnabled = true;
                if (passwordTB.Password.Equals(confirmPasswordTB.Password))
                {
                    confirmPasswordTB.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
                else
                {
                    confirmPasswordTB.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if(passwordTB.Password.Length>3)
            {
                if(passwordTB.Password.Equals(confirmPasswordTB.Password))
                {
                    if(Tools.IsValidEmail(mailTB.Text))
                    {
                        SaveChanges();
                    }
                    else
                    {
                        CustomMessageBox.ShowDialog(Window, "כתובת הדוא''ל שהוזנה אינה חוקית.", "כתובת דוא''ל לא חוקית", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                    }
                }
                else
                {
                    CustomMessageBox.ShowDialog(Window, "הסיסמאות שהוזנו אינן תואמות אחת לשנייה.", "הסיסמאות אינן תואמות", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                }
            }
            else
            {
                CustomMessageBox.ShowDialog(Window, "אורך הסיסמה חייב להיות ארוך משלושה תווים.", "הסיסמה קצרה מידי", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        private void SaveChanges()
        {
            FilteringSystem.SetAdminPassword(passwordTB.Password);
            FilteringSystem.SetAdminName(nameTB.Text);
            FilteringSystem.SetAdminMail(mailTB.Text);
            FilteringSystem.SaveChanges();
            Window.SetWelcomeLabel();
            CustomMessageBox.ShowDialog(Window, "השינויים נשמרו בהצלחה!", "השינויים נשמרו", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
        }

        private void MailTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (Tools.IsValidEmail(mailTB.Text))
                {
                    mailTB.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
                else
                {
                    mailTB.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }
    }
}
