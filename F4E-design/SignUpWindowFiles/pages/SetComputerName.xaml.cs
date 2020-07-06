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

namespace F4E_design.SignUpWindowFiles.pages
{

    /// <summary>
    /// Interaction logic for SetComputerName.xaml
    /// </summary>
    public partial class SetComputerName : Page
    {
        public string PCName;

        public SetComputerName()
        {
            InitializeComponent();
        }

        public SignUpWindow Window { get; set; }
        private static SetComputerName instance = null;
        public static SetComputerName Instance
        {
            get
            {
                if (instance == null)
                    instance = new SetComputerName();
                return instance;
            }
        }

        private void PCNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            PCName = NameTB.Text;
            Window.SetNextEnabled(NameTB.Text != "");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("fadeIn") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
        }
    }
}
