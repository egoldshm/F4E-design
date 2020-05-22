using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        private readonly SolidColorBrush BLACKLIST_COLOR = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush EXCEPTIONLIST_COLOR = new SolidColorBrush(Colors.Green);

        public class UrlRow
        {
            public string Url { get; set; }
            public string ImagePath { get; set; }
            public SolidColorBrush Color { get; set; }
        }



        List<UrlRow> Urls = new List<UrlRow>();

        private CustomListPage()
        {
            InitializeComponent();
            SetupBlacklist();
            SetupExceptionsList();
        }

        private void SetupBlacklist()
        {
            foreach(string url in FilteringSystem.GetCurrentFilteringSettings().GetCustomBlackList())
            {
                AddItemToListBox(url, false);
            }
        }
        private void SetupExceptionsList()
        {
            foreach (string url in FilteringSystem.GetCurrentFilteringSettings().GetCustomExceptionsList())
            {
                AddItemToListBox(url, true);
            }
        }

        public MainWindow Window { get; set; }
        //singelton
        private static CustomListPage instance = null;
        public static CustomListPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomListPage();
                return instance;
            }
        }


        private void ShowErrorMessage(string message)
        {
            CustomMessageBox.ShowDialog(Window, message, "הוספת האתר נכשלה", CustomMessageBox.CustomMessageBoxTypes.Error,"הבנתי");
        }

        private void AddItemToListBox(string AddedUrl,Boolean isException)
        {
            Urls.Add(new UrlRow()
            {
                Url = AddedUrl,
                Color = isException ? EXCEPTIONLIST_COLOR : BLACKLIST_COLOR,
                ImagePath = "../images/CustomListPage/delete.png"
            });

            myListView.Items.Add(Urls[Urls.Count - 1]);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Tag.ToString();
            UrlRow urlRow = Urls.FindAll(url => url.Url == content).FirstOrDefault();
            if (CustomMessageBox.ShowDialog(Window, "האם אתה בתוך שברצונך למחוק את האתר " + content + " מהרשימה?", "האם אתה בטוח?", CustomMessageBox.CustomMessageBoxTypes.Question, "מחק", "בטל"))
            {
                myListView.Items.Remove(urlRow);
                Urls.RemoveAll(item => item.Url == content);
                if(urlRow.Color==BLACKLIST_COLOR)
                {
                    FilteringSystem.RemoveSiteFromBlackList(content);
                }
                else
                {
                    FilteringSystem.RemoveSiteFromExceptionList(content);
                }
            }
        }

        private void AddToBlacklistButton_Click(object sender, RoutedEventArgs e)
        {
            string url = Tools.FormatToShortDomainUri(url_text_box.Text);
            url_text_box.Text = "";
            string result = FilteringSystem.AddSiteToBlackList(url);
            if (result == "")
            {
                AddItemToListBox(url, false);
            }
            else
                ShowErrorMessage(result);
        }

        private void AddToExceptionListButton_Click(object sender, RoutedEventArgs e)
        {
            string url = url_text_box.Text;
            url_text_box.Text = "";
            string result = FilteringSystem.AddSiteToExceptionList(url);
            if (result == "")
            {
                AddItemToListBox(url, true);
            }
            else
                ShowErrorMessage(result);
        }
    }

}
