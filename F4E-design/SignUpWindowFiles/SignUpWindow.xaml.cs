using ControlzEx;
using F4E_design.SignUpWindowFiles.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace F4E_design.SignUpWindowFiles
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {

        public SignUpWindow()
        {
            InitializeComponent();
            SetPage(0);
        }
        int part = 0;
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            part++;
            SetPage(part);
        }

        public void SetNextButtonText(string text)
        {
            nextButton.Content = text;
        }

        private void SetPage(int part)
        {
            switch (part)
            {
                case 0:
                    IntroductionPage.Instance.Window = this;
                    pagesFrame.Content = IntroductionPage.Instance;
                    SetPreviousEnabled(false);
                    break;
                case 1:
                    SetNextEnabled(false);
                    SelectPasswordPage.Instance.Window = this;
                    pagesFrame.Content = SelectPasswordPage.Instance;
                    SetPreviousEnabled(true);
                    break;
                //case 2:
                //    DotsPasswordPage.Instance.Window = this;
                //    pagesFrame.Content = DotsPasswordPage.Instance;
                //    SetNextButtonText("דלג");
                //    SetNextEnabled(true);
                //    SetPreviousEnabled(true);
                //    break;
                case 2:
                    SetComputerName.Instance.Window = this;
                    pagesFrame.Content = SetComputerName.Instance;
                    SetNextButtonText("הבא");
                    SetNextEnabled(false);
                    SetPreviousEnabled(true);
                    break;
                case 3:
                    SetMailPage.Instance.Window = this;
                    pagesFrame.Content = SetMailPage.Instance;
                    SetNextButtonText("המשך");
                    SetNextEnabled(false);
                    SetPreviousEnabled(true);
                    break;
                case 4:
                    EndPage.Instance.Window = this;
                    pagesFrame.Content = EndPage.Instance;
                    SetNextButtonText("סיום");
                    SetNextEnabled(true);
                    SetPreviousEnabled(true);
                    break;
                case 5:
                    SystemSetup();
                    this.Close();
                    break;
            }
        }

        private void SystemSetup()
        {
            try
            {
                FilteringSystem.FirstSetup();
                FilteringSystem.GetCurrentFilteringSettings().SetAdminName(IntroductionPage.Instance.enteredName);
                FilteringSystem.GetCurrentFilteringSettings().SetAdminPassword(SelectPasswordPage.Instance.enteredPassword);
                FilteringSystem.GetCurrentFilteringSettings().SetAdminMail(SetMailPage.Instance.enteredMail);
                FilteringSystem.GetCurrentFilteringSettings().SetComputerName(SetComputerName.Instance.PCName);
                FilteringSystem.SaveChanges();
                FilteringSystem.Load();
                FilteringSystem.SetSystemStatus(true);
            }
            catch(Exception e)
            {
                CustomMessageBox.ShowDialog(null, e.Message, "שגיאה בשמירת שינויים", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (part > 0)
            {
                part--;
                SetPage(part);
                if (part == 0)
                    previousButton.IsEnabled = false;
                if (part < 4)
                    nextButton.IsEnabled = true;
            }
        }

        public void SetNextEnabled(Boolean isEnabled)
        {
            nextButton.IsEnabled = isEnabled;
        }

        public void SetPreviousEnabled(Boolean isEnabled)
        {
            previousButton.IsEnabled = isEnabled;
        }
        private void PagesFrame_Navigated(object sender, NavigationEventArgs e)
        {
            pagesFrame.NavigationService.RemoveBackEntry();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.Instance.Show();
        }
    }
}
