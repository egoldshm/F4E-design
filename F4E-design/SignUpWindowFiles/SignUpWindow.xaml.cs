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
            setPage(0);
        }
        int part = 0;
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (part < 4)
            {
                part++;
                setPage(part);
                if (part == 3)
                    nextButton.IsEnabled = false;
                if (part > 0)
                    previousButton.IsEnabled = true;
            }
        }

        private void setPage(int part)
        {
            switch (part)
            {
                case 0:
                    pagesFrame.Content = new Introduction(this);
                    break;
                case 1:
                    pagesFrame.Content = new SelectPassword(this);
                    break;
                case 2:
                    //pagesFrame.Content = new Introduction();
                    break;
                case 3:
                    //pagesFrame.Content = new Introduction();
                    break;
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (part > 0)
            {
                part--;
                setPage(part);
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

        private void pagesFrame_Navigated(object sender, NavigationEventArgs e)
        {
            pagesFrame.NavigationService.RemoveBackEntry();
        }
    }
}
