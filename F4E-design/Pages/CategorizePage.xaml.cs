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
        private Boolean _isVideoPlayersBlocked;
        private Boolean _isGamesBlocked;
        private Boolean _isDatingBlocked;
        private Boolean _isViolenceBlocked;
        private Boolean _isLifeStyleBlocked;
        private Boolean _isPhotosStackBlocked;

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
            _isVideoPlayersBlocked = FilteringSystem.GetCurrentFilteringSettings().isVideoPlayersBlocked;
            _isGamesBlocked = FilteringSystem.GetCurrentFilteringSettings().isGamesBlocked;
            _isDatingBlocked = FilteringSystem.GetCurrentFilteringSettings().isDatingBlocked;
            _isViolenceBlocked= FilteringSystem.GetCurrentFilteringSettings().isViolenceBlocked;
            _isLifeStyleBlocked = FilteringSystem.GetCurrentFilteringSettings().isLifeStyleBlocked;
            _isPhotosStackBlocked= FilteringSystem.GetCurrentFilteringSettings().isPhotosStackBlocked;
            UpdateToggelsGUI();
        }

        public void UpdateToggelsGUI()
        {
            social_toggle.Source = new BitmapImage(new Uri(_isSocialNetworksBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            gambling_toggle.Source = new BitmapImage(new Uri(_isGamblingBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            news_toggle.Source = new BitmapImage(new Uri(_isNewsBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            sport_toggle.Source = new BitmapImage(new Uri(_isSportBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            videoPlayers_toggle.Source = new BitmapImage(new Uri(_isVideoPlayersBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            violence_toggle.Source = new BitmapImage(new Uri(_isViolenceBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            dating_toggle.Source = new BitmapImage(new Uri(_isDatingBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            games_toggle.Source = new BitmapImage(new Uri(_isGamesBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            lifeStyle_toggle.Source = new BitmapImage(new Uri(_isLifeStyleBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
            photosStack_toggle.Source = new BitmapImage(new Uri(_isPhotosStackBlocked ? URI_ON_IMAGE : URI_OFF_IMAGE, UriKind.Relative));
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
                case "videoPlayers_toggle":
                    _isVideoPlayersBlocked = !_isVideoPlayersBlocked;
                    break;
                case "violence_toggle":
                    _isViolenceBlocked = !_isViolenceBlocked;
                    break;
                case "dating_toggle":
                    _isDatingBlocked = !_isDatingBlocked;
                    break;
                case "games_toggle":
                    _isGamesBlocked = !_isGamesBlocked;
                    break;
                case "lifeStyle_toggle":
                    _isLifeStyleBlocked = !_isLifeStyleBlocked;
                    break;
                case "photosStack_toggle":
                    _isPhotosStackBlocked = !_isPhotosStackBlocked;
                    break;

            }
            UpdateToggelsGUI();
            SaveChangesReminderAnimation();
        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (InternetBlocker.isInternetReachable())
            {
                try
                {
                    FilteringSystem.GetCurrentFilteringSettings().isSocialNetworksBlocked = _isSocialNetworksBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isGamblingBlocked = _isGamblingBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isNewsBlocked = _isNewsBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isSportBlocked = _isSportBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isVideoPlayersBlocked = _isVideoPlayersBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isDatingBlocked = _isDatingBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isGamesBlocked = _isGamesBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isPhotosStackBlocked = _isPhotosStackBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isLifeStyleBlocked = _isLifeStyleBlocked;
                    FilteringSystem.GetCurrentFilteringSettings().isViolenceBlocked = _isViolenceBlocked;
                    FilteringSystem.SaveChanges();
                    HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
                    CustomMessageBox.ShowDialog(Window, "השינויים נשמרו בהצלחה!", "השינויים נשמרו", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
                }
                catch(Exception ex)
                {
                    CustomMessageBox.ShowDialog(Window, "כתיבת הנתונים לא הצליחה"+ Environment.NewLine + ex.Message, "שגיאה", CustomMessageBox.CustomMessageBoxTypes.Success, "המשך");
                }
            }
            else
            {
                CustomMessageBox.ShowDialog(Window, "יש צורך בחיבור לאינטרנט", "על מנת לעדכן את הקטגוריות יש צורך בחיבור לאינטרנט.", CustomMessageBox.CustomMessageBoxTypes.Stop, "הבנתי");
            }
        }
        private void SaveChangesReminderAnimation()
        {
            Storyboard sb = this.FindResource("SaveChangesReminder") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }
    }
}
