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
    /// Interaction logic for CustomListPage.xaml
    /// </summary>
    public partial class CustomListPage : Page
    {


        public class UrlRow
        {
            public int ID { get; set; }
            public string url { get; set; }
            public string imagePath { get; set; }
        }
            
        List<UrlRow> urls = new List<UrlRow>();

        public CustomListPage()
        {
            InitializeComponent();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string newUrl = url_text_box.Text;

            foreach (UrlRow url in urls)
            {
                if (url.url == newUrl)
                {
                    MessageBox.Show("כתובת אתר זו כבר קיימת ברשימה", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (newUrl != "")
            {
                AddNewUrl(newUrl);
            }
            else
            {
                MessageBox.Show("אנא קודם הכנס כתובת של אתר", "error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddNewUrl(string newUrl)
        {
            urls.Add(new UrlRow()
            {
                ID = urls.Count,
                url = newUrl,
                imagePath = "../images/CustomListPage/delete.png"
            });

            myListView.Items.Add(urls[urls.Count-1]);
        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {
            int content = Int32.Parse((sender as Button).Tag.ToString());
            MessageBoxResult result = MessageBox.Show("האם אתה בתוך שברצונך למחוק את האתר "+ urls[content].url + " מהרשימה?", "Worning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    myListView.Items.RemoveAt(content);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }

}
