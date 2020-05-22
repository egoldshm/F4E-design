using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public enum CustomMessageBoxTypes
        {
            Error,Stop,Success,Warning,Question
        }

        LinearGradientBrush redGradientBrush = new LinearGradientBrush(Color.FromRgb(224,106,91), Color.FromRgb(253, 62, 88), new Point(0.428, -0.256), new Point(0.539, 1.638));
        LinearGradientBrush greenGradientBrush = new LinearGradientBrush(Color.FromRgb(104, 189,94), Color.FromRgb(25,164,79), new Point(-0.042,0.539), new Point(1.19,0.481));
        LinearGradientBrush yellowGradientBrush = new LinearGradientBrush(Color.FromRgb(252,193,49), Color.FromRgb(243,154,53), new Point(-0.042, 0.539), new Point(1.19, 0.481));
        LinearGradientBrush blueGradientBrush = new LinearGradientBrush(Color.FromRgb(90,194,249), Color.FromRgb(86,83,224), new Point(-0.042, 0.539), new Point(1.19, 0.481));

        public CustomMessageBox(Window windowSender ,string text, string title, CustomMessageBoxTypes type,string yesText,string noText)
        {
            InitializeComponent();
            this.Title = title;
            this.Owner = windowSender;
            if (windowSender == null)
            {
                this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            titleLabel.Text= title;
            textLabel.Text = text;
            LinearGradientBrush typeColor=null;
            string iconUri = "";
            NoButton.Content = noText;
            YesButton.Content = yesText;
            if (noText == "")
                setJustOkButton();
            switch (type)
            {
                case CustomMessageBoxTypes.Error:
                    typeColor = redGradientBrush;
                    iconUri = "/images/CustomMessageBox/errorIcon.png";
                    break;
                case CustomMessageBoxTypes.Stop:
                    typeColor = redGradientBrush;
                    iconUri = "/images/CustomMessageBox/stopIcon.png";
                    break;
                case CustomMessageBoxTypes.Warning:
                    typeColor = yellowGradientBrush;
                    iconUri = "/images/CustomMessageBox/warningIcon.png";
                    break;
                case CustomMessageBoxTypes.Question:
                    typeColor = blueGradientBrush;
                    iconUri = "/images/CustomMessageBox/questionIcon.png";
                    break;
                case CustomMessageBoxTypes.Success:
                    typeColor = greenGradientBrush;
                    iconUri = "/images/CustomMessageBox/successIcon.png";
                    break;
            }
            TopBar.Background = typeColor;
            strip.Fill = typeColor;
            titleLabel.Foreground = typeColor;
            iconImage.Source = new BitmapImage(new Uri(iconUri, UriKind.Relative));
        }

        public static Boolean ShowDialog(Window windowSender, string text, string title, CustomMessageBoxTypes type,string yesText)
        {
            CustomMessageBox customMessage = new CustomMessageBox(windowSender, text, title, type,yesText,"");
            return (Boolean)customMessage.ShowDialog();
        }

        public static Boolean ShowDialog(Window windowSender, string text, string title, CustomMessageBoxTypes type,string yseText,string noText)
        {
            CustomMessageBox customMessage = new CustomMessageBox(windowSender, text, title, type, yseText,noText);
            return (Boolean)customMessage.ShowDialog();
        }

        private void setJustOkButton()
        {
            spaceBetweenButtonsColumn.Width = new GridLength(22.8);
            noButtonColumn.Width = new GridLength(0);
        }
        
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
