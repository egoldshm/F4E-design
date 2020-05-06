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
    /// Interaction logic for StatusPage.xaml
    /// </summary>
    public partial class StatusPage : Page
    {
        public StatusPage()
        {
            InitializeComponent();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
        }
        private void sceduelToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string URI_ON_IMAGE = "/images/statusPage/scheduel_on.png";
            const string URI_OFF_IMAGE = "/images/statusPage/scheduel_off.png";
            if (scheduelToggle.Name.EndsWith("Active"))
            {
                scheduelToggle.Source = new BitmapImage(new Uri(URI_ON_IMAGE, UriKind.Relative));
                scheduelToggle.Name = scheduelToggle.Name.Replace("Active", "");
            }
            else
            {
                scheduelToggle.Source = new BitmapImage(new Uri(URI_OFF_IMAGE, UriKind.Relative));
                scheduelToggle.Name = scheduelToggle.Name != null ? scheduelToggle.Name + "Active" : "Active";
            }
        }

        private void systemStatusToggle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string URI_ON_IMAGE = "/images/statusPage/status_on.png";
            const string URI_OFF_IMAGE = "/images/statusPage/status_off.png";
            if (systemStatusToggle.Name.EndsWith("Active"))
            {
                systemStatusToggle.Source = new BitmapImage(new Uri(URI_ON_IMAGE, UriKind.Relative));
                systemStatusToggle.Name = scheduelToggle.Name.Replace("Active", "");
            }
            else
            {
                systemStatusToggle.Source = new BitmapImage(new Uri(URI_OFF_IMAGE, UriKind.Relative));
                systemStatusToggle.Name = scheduelToggle.Name != null ? scheduelToggle.Name + "Active" : "Active";
            }
        }
    }
}
