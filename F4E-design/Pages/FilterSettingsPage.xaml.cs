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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F4E_design.Pages
{
    /// <summary>
    /// Interaction logic for FilterSettingsPage.xaml
    /// </summary>
    public partial class FilterSettingsPage : Page
    {
        private const string URI_ON_IMAGE = "/images/filtersettingspage/on.png";
        private const string URI_OFF_IMAGE = "/images/filtersettingspage/off.png";


        private Boolean SafeServer;
        private Boolean AdBlock;

        private FilterSettingsPage()
        {
            InitializeComponent();
            ResetGUI();
        }

        public MainWindow Window { get; set; }
        //singelton
        private static FilterSettingsPage instance = null;
        public static FilterSettingsPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new FilterSettingsPage();
                return instance;
            }
        }
        
        public void ResetGUI()
        {
            SafeServer = FilteringSystem.GetCurrentFilteringSettings().isSafeServerOn;
            AdBlock = FilteringSystem.GetCurrentFilteringSettings().isAdBlockOn;
            filteringLevelComboBox.SelectedIndex =(int)FilteringSystem.GetCurrentFilteringSettings()._youtubeFilteringLevel;
            UpdatePageGUI();
        }
       
        private bool handle = true;
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle(sender);
            handle = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle(sender);
            if (SafeServer&& cmb.SelectedIndex == 0)
            {
                CustomMessageBox.ShowDialog(Window, "לא ניתן לאפשר גלישה חופשית ביוטיוב כאשר השרת הבטוח פעיל.על מנת לאפשר אופציה זו יש לכבות את השרת הבטוח.", "שרת בטוח מופעל", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                cmb.SelectedIndex = 1;
            }
            else
            {
                if (this.IsLoaded)
                    SaveChangesReminderAnimation();
            }
        }

        private void Handle(object sender)
        {
            ComboBox comboBox = sender as ComboBox;
            var selected = comboBox.SelectedItem as ComboBoxItem;
            if (selected != null)
                comboBox.Background = selected.Background;


        }

        private void SafeServerToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SafeServer = !SafeServer;
            if (SafeServer)
                filteringLevelComboBox.SelectedIndex = 1;
            SaveChangesReminderAnimation();
            UpdatePageGUI();
        }
        private void AdBlockToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AdBlock = !AdBlock;
            SaveChangesReminderAnimation();
            UpdatePageGUI();
        }

        private void UpdatePageGUI()
        {
            safeServerToggle.Source = new BitmapImage(new Uri(SafeServer ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            FilteringSystem.GetCurrentFilteringSettings().isSafeServerOn = SafeServer;
            FilteringSystem.GetCurrentFilteringSettings().isAdBlockOn = AdBlock;
            FilteringSystem.GetCurrentFilteringSettings()._youtubeFilteringLevel =(FilteringSettings.YoutubeFilteringLevels) filteringLevelComboBox.SelectedIndex;
            FilteringSystem.SaveChanges();
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            CustomMessageBox.ShowDialog(Window, "השינויים נשמרו בהצלחה!", "הגדרות סינון", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
        }

        private void SaveChangesReminderAnimation()
        {
            Storyboard sb = this.FindResource("SaveChangesReminder") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }
       
    }
}
