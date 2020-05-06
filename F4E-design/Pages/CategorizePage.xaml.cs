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

        public CategorizePage()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image.Name.EndsWith("Active"))
            {
                image.Source = new BitmapImage(new Uri(URI_ON_IMAGE, UriKind.Relative));
                image.Name = image.Name.Replace("Active", "");
            }
            else
            {
                image.Source = new BitmapImage(new Uri(URI_OFF_IMAGE, UriKind.Relative));
                image.Name = image.Name != null ? image.Name + "Active" : "Active";
            }
        }
    }
}
