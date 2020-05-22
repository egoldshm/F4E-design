using F4E_design.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ReplacePage(current_status, null);
            SetWelcomeLabel();
        }

        private void SetWelcomeLabel()
        {
            int hour = DateTime.Now.Hour;
            string welcomeText = "";
            if (hour >= 0 && hour <= 4)
            {
                welcomeText = "לילה טוב לך, ";
            }
            if (hour >= 5 && hour <=11)
            {
                welcomeText = "בוקר טוב לך, ";
            }
            if (hour >= 12 && hour <= 15)
            {
                welcomeText = "צהריים טובים לך, ";
            }
            if (hour >= 16 && hour <= 19)
            {
                welcomeText = "אחר הצהריים טובים, ";
            }
            if (hour >= 20 && hour <= 23)
            {
                welcomeText = "לילה טוב לך, ";
            }
            welcomeLabel.Content = welcomeText + FilteringSystem.GetAdminName();
        }

        private static MainWindow instance = null;
        public static MainWindow Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainWindow();
                return instance;
            }
        }
        
        private void ReplacePage(object sender, MouseButtonEventArgs e)
        {
            Label obj = sender as Label;
            ChooseTheNewLabel(obj);
            switch (obj.Name)
            {
                case "current_status":
                    FrameWindow.Content = StatusPage.Instance;
                    break;
                case "filtering_settings":
                    FilterSettingsPage.Instance.ResetGUI();
                    FilterSettingsPage.Instance.Window = this;
                    FrameWindow.Content = FilterSettingsPage.Instance;
                    break;
                case "categorize":
                    CategorizePage.Instance.ResetToggles();
                    CategorizePage.Instance.Window = this;
                    FrameWindow.Content = CategorizePage.Instance;
                    break;
                case "custom_list":
                    CustomListPage.Instance.Window = this;
                    FrameWindow.Content = CustomListPage.Instance;
                    break;
                case "general_settings":
                    FrameWindow.Content = GeneralSettingsPage.Instance;
                    break;
                case "about":
                    FrameWindow.Content = AboutPage.Instance;
                    break;
                case "scheduel":
                    SchedulePage.Instance.Window = this;
                    FrameWindow.Content = SchedulePage.Instance;
                    break;
            }
        }

        private void ChooseTheNewLabel(Label obj)
        {
            foreach (Label child in Menu.Children.OfType<Label>())
            {
                child.BorderThickness = new Thickness(0, 0, 0, 0);
                child.FontFamily = new FontFamily("Assistant");
                child.FontWeight = FontWeights.Normal;
            }
            foreach (Label child in BottomMenu.Children.OfType<Label>())
            {
                child.BorderThickness = new Thickness(0, 0, 0, 0);
                child.FontFamily = new FontFamily("Assistant");
                child.FontWeight = FontWeights.Normal;
            }
            obj.BorderThickness = new Thickness(0, 0, 2.5, 0);
            obj.FontFamily = new FontFamily("Assistant-Bold");
            obj.FontWeight = FontWeights.UltraBold;
        }

        private void current_status_MouseMove(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.Foreground = new SolidColorBrush(Colors.LightGray);
        }

        private void current_status_MouseLeave(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.Foreground = new SolidColorBrush(Colors.White);
        }

        private void BottomMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Boolean clickedYes = CustomMessageBox.ShowDialog(this, "שלום!", "אני עובד!", CustomMessageBox.CustomMessageBoxTypes.Question,"אכן כן");
            string resultString = clickedYes ? "כן" : "לא";
            MessageBox.Show("המשתמש לחץ על " + resultString);
        }
    
        public void lockWindow(object sender, RoutedEventArgs e)
        {
            EnterAdminPasswordWindow enterAdminPassword = new EnterAdminPasswordWindow(this);
            enterAdminPassword.ShowDialog();
        }

        private void exit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!CustomMessageBox.ShowDialog(this, "התנתקות תוביל לסגירה מוחלטת של מערכת הסינון ותוביל לכך שהמחשב יהיה לא מסונן. האם אתה בטוח שברצונך לצאת?", "שים לב!", CustomMessageBox.CustomMessageBoxTypes.Stop, "בטל", "התנתק"))
            {
                Application.Current.Shutdown();
            }
        }

        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
