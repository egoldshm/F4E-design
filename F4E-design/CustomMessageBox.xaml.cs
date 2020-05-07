using System;
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

        public CustomMessageBox(string text, string title, CustomMessageBoxTypes type)
        {
            InitializeComponent();
            titleLabel.Content= title;
            textLabel.Text = text;
            LinearGradientBrush typeColor=null;
            string iconUri = "";
            switch (type)
            {
                case CustomMessageBoxTypes.Error:
                    typeColor = redGradientBrush;
                    iconUri = "/images/CustomMessageBox/errorIcon.png";
                    setJustOkButton();
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
                    setJustOkButton();
                    break;
            }
            TopBar.Background = typeColor;
            strip.Fill = typeColor;
            titleLabel.Foreground = typeColor;
            iconImage.Source = new BitmapImage(new Uri(iconUri, UriKind.Relative));
        }

        public static Boolean ShowDialog(string text, string title, CustomMessageBoxTypes type)
        {
            CustomMessageBox customMessage = new CustomMessageBox(text, title, type);
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
