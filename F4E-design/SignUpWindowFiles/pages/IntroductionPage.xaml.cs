using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace F4E_design.SignUpWindowFiles.pages
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class IntroductionPage : Page
    {
        public string enteredName;

        private IntroductionPage()
        {
            InitializeComponent();
        }

        //singelton
        public SignUpWindow Window { get; set; }
      
        private static IntroductionPage instance = null;
        public static IntroductionPage Instance { get
            {
                if (instance == null)
                    instance = new IntroductionPage();
                return instance;
            } }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeIn") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameTB.Text.Length > 0)
            {
                enteredName = NameTB.Text;
                Window.SetNextEnabled(true);
            }
            else
            {
                Window.SetNextEnabled(false);
            }
        }
    }
}
