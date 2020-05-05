using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Runtime.InteropServices;

namespace F4E_design.Pages
{
    /// <summary>
    /// Interaction logic for SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {

        public static readonly Brush COLOR_OF_SELECTED_BUTTON = new SolidColorBrush(Color.FromRgb(232, 167, 149));
        public static readonly Brush COLOR_OF_UNSELECTED_BUTTON = Brushes.LightGray;

        static private bool[,] tableOfHours = new bool[7, 48];

        public SchedulePage()
        {
            InitializeComponent();
            DefineTableOfSchedule();
            TableOfHours[0, 5] = true;
            FromArrayOfBoolToButton(TableOfHours);
        }

        public bool[,] TableOfHours { get => tableOfHours; set => tableOfHours = value; }

        private void DefineTableOfSchedule()
        {
            var converter = new System.Windows.Media.BrushConverter();
            for (int j = 0; j < 7; j++)
            {
                string days = "אבגדהוז";
                Button newButton = new Button();
                newButton.SetValue(Grid.RowProperty, 0);
                newButton.SetValue(Grid.ColumnProperty, j + 1);
                newButton.Name = "day_button" + j;
                //newButton.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                newButton.BorderThickness = new Thickness(0, 0, 0, 0);
                newButton.Background = (Brush)converter.ConvertFromString("#FFCBD8E6");
                newButton.FontFamily = new FontFamily("Assistant Bold");
                newButton.FontWeight = FontWeights.Bold;
                newButton.FontSize = 13;
                newButton.MouseMove += dayButton_MouseMove;
                newButton.GotMouseCapture += Button_GotMouseCapture;
                newButton.PreviewMouseDown+= dayButton_PreviewMouseDown;
                newButton.Content = days[j];
                ScheduleGrid.Children.Add(newButton);
            }

            for (int i = 0; i < 48; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                ScheduleGrid.RowDefinitions.Add(rowDefinition);
                Label hourLabel = new Label();
                string stringForLabel = i / 2 + ":" + (i % 2 == 0 ? "00" : "30");
                hourLabel.Content = stringForLabel;
                hourLabel.SetValue(Grid.RowProperty, i + 1);
                hourLabel.SetValue(Grid.ColumnProperty, 0);
                hourLabel.FontFamily = new FontFamily("Assistant");
                hourLabel.FontSize = 8;
                hourLabel.MouseLeftButtonUp += ChooseHour;
                hourLabel.MouseRightButtonDown += UnselectedHour;
                ScheduleGrid.Children.Add(hourLabel);
                for (int j = 0; j < 7; j++)
                {
                    Button newButton = new Button();
                    newButton.SetValue(Grid.RowProperty, i + 1);
                    newButton.SetValue(Grid.ColumnProperty, j + 1);
                    newButton.Name = "button_" + i + "_" + j;
                    this.RegisterName(newButton.Name, newButton);
                    //newButton.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                    newButton.BorderThickness = new Thickness(0, 0, 0, 0);
                    newButton.Background = Brushes.White;
                    newButton.PreviewMouseDown += singleHour_PreviewMouseDown;
                    newButton.MouseMove += singleHour_MouseMove;
                    newButton.GotMouseCapture += Button_GotMouseCapture;
                    ScheduleGrid.Children.Add(newButton);
                }

                
            }

        }

        private void dayButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dayButton_MouseMove(sender, null);
        }

        private void singleHour_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            singleHour_MouseMove(sender, null);
        }

        private void dayButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                ChooseFullDay(sender, null);
            }
            if (Mouse.RightButton.Equals(MouseButtonState.Pressed))
            {
                UnChooseFullDay(sender, null);
            }
        }

        private void Button_GotMouseCapture(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.ReleaseMouseCapture();
        }

        private void UnChooseFullDay(object sender, MouseButtonEventArgs e)
        {
            Button buttonSender = sender as Button;
            int day = (int)buttonSender.GetValue(Grid.ColumnProperty) - 1;
            for (int i = 0; i < 48; i++)
            {
                Button button = ScheduleGrid.FindName("button_" + i + "_" + day) as Button;
                button.Background = COLOR_OF_UNSELECTED_BUTTON;
                tableOfHours[day, i] = false;

            }

        }

        private void ChooseFullDay(object sender, RoutedEventArgs e)
        {
            Button buttonSender = sender as Button;
            int day = (int)buttonSender.GetValue(Grid.ColumnProperty) - 1;
            for (int i = 0; i < 48; i++)
            {
                Button button = ScheduleGrid.FindName("button_" + i + "_" + day) as Button;
                button.Background = COLOR_OF_SELECTED_BUTTON;
                tableOfHours[day, i] = true;

            }


        }
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void singleHour_MouseMove(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                Button button = sender as Button;
                int day = 7 - (int)button.GetValue(Grid.ColumnProperty);
                int hour = (int)button.GetValue(Grid.RowProperty) - 1;
                button.Background = COLOR_OF_SELECTED_BUTTON;
                tableOfHours[day, hour] = true;
            }

            if (Mouse.RightButton.Equals(MouseButtonState.Pressed))
            {
                Button button = sender as Button;
                int day = 7 - (int)button.GetValue(Grid.ColumnProperty);
                int hour = (int)button.GetValue(Grid.RowProperty) - 1;
                button.Background = COLOR_OF_UNSELECTED_BUTTON;
                tableOfHours[day, hour] = false;
            }
        }

        private void SaveChanges(object sender, MouseButtonEventArgs e)
        {
            //ToDo: to keep the Schedule in keepable place
        }

        private void FromArrayOfBoolToButton(bool[,] arr)
        {
            for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < 48; i++)
                {
                    object buttonObj = ScheduleGrid.FindName("button_" + i + "_" + j);
                    if (buttonObj is Button)
                    {
                        Button button = buttonObj as Button;
                        if (arr[j, i])
                        {
                            button.Background = COLOR_OF_SELECTED_BUTTON;
                        }
                        else
                        {
                            button.Background = COLOR_OF_UNSELECTED_BUTTON;
                        }
                    }
                }
            }
        }

    }
}
