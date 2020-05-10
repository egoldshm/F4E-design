using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using WpfAnimatedGif;

namespace F4E_design.SignUpWindowFiles.pages
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : Page
    {
        System.Windows.Threading.DispatcherTimer gifStarter = new System.Windows.Threading.DispatcherTimer();
        SignUpWindow _window;
        public Introduction(SignUpWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private void GifStarter_Tick(object sender, EventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(gifViewer);
            controller.Play();
            Storyboard sb = this.FindResource("fadeIn") as Storyboard;
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Begin();
            gifStarter.Stop();
        }

        private void gifViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"/signupwindowfiles/images/screen0.gif", UriKind.RelativeOrAbsolute);
                image.EndInit();
                ImageBehavior.SetAutoStart(gifViewer, false);
                ImageBehavior.SetAnimatedSource(gifViewer, image);
                ImageBehavior.SetRepeatBehavior(gifViewer, new RepeatBehavior(1));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {  
            gifStarter.Interval = new TimeSpan(0, 0, 2);
            gifStarter.Tick += GifStarter_Tick;
            gifStarter.Start();
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            _window.SetNextEnabled(NameTB.Text.Length > 0 ? true:false);
        }
    }
}
