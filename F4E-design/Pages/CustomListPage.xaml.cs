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
            if (urls.Exists((url) => url.url == newUrl))
            {
                    ErrorMessageLabel.Content = "כתובת אתר זו כבר קיימת ברשימה";
                    return;
            }

            if (newUrl != "")
            {
                AddNewUrl(newUrl);
            }
            else
            {
                ErrorMessageLabel.Content = "אנא קודם הכנס כתובת של אתר";
           }
        }

        private void AddNewUrl(string newUrl)
        {
            urls.Add(new UrlRow()
            {
                url = newUrl,
                imagePath = "../images/CustomListPage/delete.png"
            });

            myListView.Items.Add(urls[urls.Count-1]);
        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Tag.ToString();
            MessageBoxResult result = MessageBox.Show("האם אתה בתוך שברצונך למחוק את האתר "+ content + " מהרשימה?", "Worning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            UrlRow urlRow = urls.FindAll(url => url.url == content).FirstOrDefault();
            switch (result)
            {
                case MessageBoxResult.Yes:
                    myListView.Items.Remove(urlRow);
                    urls.RemoveAll(item => item.url == content);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Url_text_box_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
        }
    }

}
