using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace F4E_design.SignUpWindowFiles.pages
{
    /// <summary>
    /// Interaction logic for SetMailPage.xaml
    /// </summary>
    public partial class SetMailPage : Page
    {
        public string enteredMail;

        public SetMailPage()
        {
            InitializeComponent();
        }

        //singelton
        public SignUpWindow Window { get; set; }
        private static SetMailPage instance = null;
        public static SetMailPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new SetMailPage();
                return instance;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeIn") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }

        private void MailTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Tools.IsValidEmail(MailTB.Text))
            {
                enteredMail = MailTB.Text;
                Window.SetNextEnabled(true);
            }
            else
            {
                Window.SetNextEnabled(false);
            }
        }
       
    }
}
