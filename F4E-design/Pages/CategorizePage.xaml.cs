using F4E_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for CategorizePage.xaml
    /// </summary>
    public partial class CategorizePage : Page
    {
        //(const = readonly static)
        private const string URI_ON_IMAGE = "/images/categorizepage/on.png";
        private const string URI_OFF_IMAGE = "/images/categorizepage/off.png";

        private Boolean _isGamblingBlocked;
        private Boolean _isSocialNetworksBlocked;
        private Boolean _isNewsBlocked;
        private Boolean _isSportBlocked;
        private Boolean _isPlayersAndPicturesBlocked;

        private CategorizePage()
        {
            InitializeComponent();
            ResetToggles();
        }

        public MainWindow Window { get; set; }
        //singelton
        private static CategorizePage instance = null;
        public static CategorizePage Instance
        {
            get
            {
                if (instance == null)
                    instance = new CategorizePage();
                return instance;
            }
        }

        public void ResetToggles()
        {
            _isSocialNetworksBlocked = FilteringSystem.GetCurrentFilteringSettings().isSocialNetworksBlocked;
            _isGamblingBlocked = FilteringSystem.GetCurrentFilteringSettings().isGamblingBlocked;
            _isNewsBlocked = FilteringSystem.GetCurrentFilteringSettings().isNewsBlocked;
            _isSportBlocked = FilteringSystem.GetCurrentFilteringSettings().isSportBlocked;
            _isPlayersAndPicturesBlocked = FilteringSystem.GetCurrentFilteringSettings().isPlayersAndPicturesBlocked;
            UpdateToggelsGUI();
        }

        public void UpdateToggelsGUI()
        {
            social_toggle.Source = new BitmapImage(new Uri(_isSocialNetworksBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            gambling_toggle.Source = new BitmapImage(new Uri(_isGamblingBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            news_toggle.Source = new BitmapImage(new Uri(_isNewsBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            sport_toggle.Source = new BitmapImage(new Uri(_isSportBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            players_toggle.Source = new BitmapImage(new Uri(_isPlayersAndPicturesBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            switch(image.Name)
            {
                case "social_toggle":
                    _isSocialNetworksBlocked = !_isSocialNetworksBlocked;
                    break;
                case "gambling_toggle":
                    _isGamblingBlocked = !_isGamblingBlocked;
                    break;
                case "news_toggle":
                    _isNewsBlocked = !_isNewsBlocked;
                    break;
                case "sport_toggle":
                    _isSportBlocked = !_isSportBlocked;
                    break;
                case "players_toggle":
                    _isPlayersAndPicturesBlocked = !_isPlayersAndPicturesBlocked;
                    break;
            }
            UpdateToggelsGUI();
            SaveChangesReminderAnimation();
        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            FilteringSystem.GetCurrentFilteringSettings().isSocialNetworksBlocked = _isSocialNetworksBlocked;
            FilteringSystem.GetCurrentFilteringSettings().isGamblingBlocked = _isGamblingBlocked;
            FilteringSystem.GetCurrentFilteringSettings().isNewsBlocked = _isNewsBlocked;
            FilteringSystem.GetCurrentFilteringSettings().isSportBlocked = _isSportBlocked;
            FilteringSystem.GetCurrentFilteringSettings().isPlayersAndPicturesBlocked = _isPlayersAndPicturesBlocked;
            FilteringSystem.SaveChanges();
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            CustomMessageBox.ShowDialog(Window, "השינויים נשמרו בהצלחה!", "קטגוריות סינון", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
        }
        private void SaveChangesReminderAnimation()
        {
            Storyboard sb = this.FindResource("SaveChangesReminder") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }
    }
}
