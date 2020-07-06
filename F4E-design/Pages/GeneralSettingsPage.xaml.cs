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
            nameTB.Text = FilteringSystem.GetCurrentFilteringSettings().GetAdminName();
            passwordTB.Password = PasswordEncryption.Decrypt(FilteringSystem.GetCurrentFilteringSettings().GetAdminPassword());
            confirmPasswordTB.Password = passwordTB.Password;
            confirmPasswordTB.Foreground = new SolidColorBrush(Colors.LimeGreen);
            pcNameTB.Text = FilteringSystem.GetCurrentFilteringSettings().GetComputerName();
            mailTB.Text = FilteringSystem.GetCurrentFilteringSettings().GetAdminMail();
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
            if (nameTB.Text != "")
            {
                if (pcNameTB.Text != "")
                {
                    if (passwordTB.Password.Length > 3)
                    {
                        if (passwordTB.Password.Equals(confirmPasswordTB.Password))
                        {
                            if (Tools.IsValidEmail(mailTB.Text))
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
                else
                {
                    CustomMessageBox.ShowDialog(Window, "שם המחשב אינו יכול להיות ריק.", "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                }
            }
            else
            {
                CustomMessageBox.ShowDialog(Window, "שם מנהל המערכת אינו יכול להיות ריק.", "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        private void SaveChanges()
        {
            Boolean savedSuccessfuly = false;
            int attempts = 0;
            Exception error = null;
            while (!savedSuccessfuly)
            {
                if (attempts < 4)
                {
                    try
                    {
                        FilteringSystem.GetCurrentFilteringSettings().SetAdminPassword(passwordTB.Password);
                        FilteringSystem.GetCurrentFilteringSettings().SetAdminName(nameTB.Text);
                        FilteringSystem.GetCurrentFilteringSettings().SetAdminMail(mailTB.Text);
                        FilteringSystem.GetCurrentFilteringSettings().SetComputerName(pcNameTB.Text);
                        FilteringSystem.SaveChanges();
                        Window.SetWelcomeLabel();
                        savedSuccessfuly = true;
                        CustomMessageBox.ShowDialog(Window, "השינויים נשמרו בהצלחה!", "השינויים נשמרו", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
                    }
                    catch (Exception e)
                    {
                        error = e;
                        attempts++;
                        System.Threading.Thread.Sleep(200);
                    }
                }
                else
                {
                    CustomMessageBox.ShowDialog(null, error.Message, "שגיאה בשמירת נתונים", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                    break;
                }
            }
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
