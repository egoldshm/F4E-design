using F4E_design.Pages;
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

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

      
        //X button - close the window
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ReplacePage(object sender, MouseButtonEventArgs e)
        {
            Label obj = sender as Label;
            ChooseTheNewLabel(obj);
            switch (obj.Name)
            {
                case "page1":
                    FrameWindow.Content = new StatusPage();
                    break;
                case "page2":
                    FrameWindow.Content = new Page1();
                    break;
                case "page3":
                    FrameWindow.Content = new SchedulePage();
                    break;
                default:
                    break;
            }
        }

        private void ChooseTheNewLabel(Label obj)
        {
            foreach (Label child in Menu.Children.OfType<Label>())
            {
                child.FontFamily = new FontFamily("Assistant");
                child.FontWeight = FontWeights.Normal;

            }
            obj.FontFamily = new FontFamily("Assistant-Bold");
            obj.FontWeight = FontWeights.UltraBold;
        }
    }
}
