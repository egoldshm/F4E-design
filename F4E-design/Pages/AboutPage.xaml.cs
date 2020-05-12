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
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        private AboutPage()
        {
            InitializeComponent();
        }

        //singelton
        private static AboutPage instance = null;
        public static AboutPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new AboutPage();
                return instance;
            }
        }


    }
}
