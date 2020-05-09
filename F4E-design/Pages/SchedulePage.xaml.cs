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

namespace F4E_design.Pages
{
    /// <summary>
    /// Interaction logic for SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {

        public static readonly Brush COLOR_OF_SELECTED_BUTTON = new SolidColorBrush(Color.FromRgb(232, 167, 149));
        public static readonly Brush COLOR_OF_UNSELECTED_BUTTON = Brushes.LightGray;
        public static readonly Brush COLOR_OF_DAY_BUTTON = (Brush)new BrushConverter().ConvertFromString("#FFCBD8E6");
        public static readonly Brush COLOR_OF_SIGN_DAY_AND_HOUR = (Brush)new BrushConverter().ConvertFromString("#82B1CB");
        const int NUM_OF_DAYS = 7;
        const int NUM_OF_HOURS = 48;

        static private bool[,] tableOfHours = new bool[NUM_OF_DAYS, NUM_OF_HOURS];

        /* Notice:
         * Row of Button = Hour
         * Column of Button = Day + 1
         * */

        public SchedulePage()
        {
            InitializeComponent();
            DefineTableOfSchedule();
            FromArrayOfBoolToButton(TableOfHours);
            //getButtonByDateTime(DateTime.Now).Background = COLOR_OF_SELECTED_BUTTON;
            //ScrollArea.ScrollToEnd(); //לדעתי יותר שימושי שהחלון יתחיל מלמטה - כי שעות הערב הרבה יותר שימושיות
        }

        public bool[,] TableOfHours { get => tableOfHours; set => tableOfHours = value; }

        private void DefineTableOfSchedule()
        {
            //days button
            string days = "אבגדהוש";
            for (int j = 0; j < NUM_OF_DAYS; j++)
            {
                Button newButton = new Button();
                newButton.SetValue(Grid.RowProperty, 0);
                newButton.SetValue(Grid.ColumnProperty, j + 1);
                newButton.Name = "day_button" + j;
                ScheduleGrid.RegisterName(newButton.Name, newButton);
                //newButton.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                newButton.BorderThickness = new Thickness(0, 0, 0, 0);
                newButton.Background = COLOR_OF_DAY_BUTTON;
                newButton.FontFamily = new FontFamily("Assistant Bold");
                newButton.FontWeight = FontWeights.Bold;
                newButton.FontSize = 13;
                newButton.MouseMove += dayButton_MouseMove;
                newButton.GotMouseCapture += Button_GotMouseCapture;
                newButton.PreviewMouseDown += dayButton_PreviewMouseDown;
                newButton.Content = days[j];
                newButton.ToolTip = "יום " + days[j];
                ScheduleTopGrid.Children.Add(newButton);
            }

            //define all hours
            for (int i = 0; i < NUM_OF_HOURS; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                ScheduleGrid.RowDefinitions.Add(rowDefinition);

                //added weekly hour button
                Button weeklyHour = new Button();
                string hourString = i / 2 + ":" + (i % 2 == 0 ? "00" : "30");
                weeklyHour.Content = hourString;
                weeklyHour.Name = "WeeklyHour" + i;
                ScheduleGrid.RegisterName(weeklyHour.Name, weeklyHour);
                weeklyHour.BorderThickness = new Thickness(0);
                weeklyHour.Background = new SolidColorBrush(Colors.White) { Opacity = 0 };
                weeklyHour.SetValue(Grid.RowProperty, i);
                weeklyHour.SetValue(Grid.ColumnProperty, 0);
                weeklyHour.FontFamily = new FontFamily("Assistant Bold");
                weeklyHour.FontSize = 10;
                //weeklyHour.FontWeight = FontWeights.Bold;
                weeklyHour.GotMouseCapture += Button_GotMouseCapture;
                weeklyHour.MouseMove += WeeklyHour_MouseMove;
                weeklyHour.PreviewMouseDown += WeeklyHour_PreviewMouseDown;
                weeklyHour.Margin = new Thickness(0);
                ScheduleGrid.Children.Add(weeklyHour);
                //add all the button for single hour
                for (int j = 0; j < NUM_OF_DAYS; j++)
                {
                    Button singleHour = new Button();
                    singleHour.SetValue(Grid.RowProperty, i);
                    singleHour.SetValue(Grid.ColumnProperty, j + 1);
                    singleHour.Name = "button_" + i + "_" + j;
                    this.RegisterName(singleHour.Name, singleHour);
                    //singleHour.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                    singleHour.BorderThickness = new Thickness(0, 0, 0, 0);
                    singleHour.Background = Brushes.White;
                    singleHour.PreviewMouseDown += singleHour_PreviewMouseDown;
                    singleHour.MouseMove += singleHour_MouseMove;
                    singleHour.GotMouseCapture += Button_GotMouseCapture;
                    singleHour.ToolTip = "יום " + days[j] + ", " + hourString;

                    ScheduleGrid.Children.Add(singleHour);
                }


            }

        }


        private void WeeklyHour_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WeeklyHour_MouseMove(sender, null);
        }

        private void WeeklyHour_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                ChooseWeeklyHour(sender, null);
            }
            if (Mouse.RightButton.Equals(MouseButtonState.Pressed))
            {
                UnChooseWeeklyHour(sender, null);
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

        private void UnChooseWeeklyHour(object sender, MouseButtonEventArgs e)
        {
            Button hourSender = sender as Button;
            int hour = (int)hourSender.GetValue(Grid.RowProperty);
            for (int i = 0; i < NUM_OF_DAYS; i++)
            {
                Button button = ScheduleGrid.FindName("button_" + hour + "_" + i) as Button;
                button.Background = COLOR_OF_UNSELECTED_BUTTON;
                tableOfHours[i, hour] = false;
            }
        }

        private void ChooseWeeklyHour(object sender, MouseButtonEventArgs e)
        {
            Button hourSender = sender as Button;
            int hour = (int)hourSender.GetValue(Grid.RowProperty);
            for (int i = 0; i < NUM_OF_DAYS; i++)
            {
                Button button = ScheduleGrid.FindName("button_" + hour + "_" + i) as Button;
                button.Background = COLOR_OF_SELECTED_BUTTON;
                tableOfHours[i, hour] = true;

            }
        }

        private void UnChooseFullDay(object sender, MouseButtonEventArgs e)
        {
            Button buttonSender = sender as Button;
            int day = (int)buttonSender.GetValue(Grid.ColumnProperty) - 1;
            for (int i = 0; i < NUM_OF_HOURS; i++)
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
            for (int i = 0; i < NUM_OF_HOURS; i++)
            {
                Button button = ScheduleGrid.FindName("button_" + i + "_" + day) as Button;
                button.Background = COLOR_OF_SELECTED_BUTTON;
                tableOfHours[day, i] = true;
            }
        }

        private void singleHour_MouseMove(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.CaptureMouse();

            Button button = sender as Button;
            int day = (int)button.GetValue(Grid.ColumnProperty) - 1;
            int hour = (int)button.GetValue(Grid.RowProperty);
            for (int i = 0; i < NUM_OF_DAYS; i++)
            {
                Button tempDayButton = ScheduleGrid.FindName("day_button" + i) as Button;
                tempDayButton.Background = COLOR_OF_DAY_BUTTON;
            }
            for (int i = 0; i < NUM_OF_HOURS; i++)
            {
                Button tempHourButton = ScheduleGrid.FindName("WeeklyHour" + i) as Button;
                tempHourButton.Background = new SolidColorBrush(Colors.White) { Opacity = 0 }; ;

            }
            //sign the hour button
            Button hourButton = ScheduleGrid.FindName("WeeklyHour" + hour) as Button;
            hourButton.Background = COLOR_OF_SIGN_DAY_AND_HOUR;

            Button dayButton = ScheduleGrid.FindName("day_button" + day) as Button;
            dayButton.Background = COLOR_OF_SIGN_DAY_AND_HOUR;

            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                button.Background = COLOR_OF_SELECTED_BUTTON;
                tableOfHours[day, hour] = true;
            }

            if (Mouse.RightButton.Equals(MouseButtonState.Pressed))
            {
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
            for (int j = 0; j < NUM_OF_DAYS; j++)
            {
                for (int i = 0; i < NUM_OF_HOURS; i++)
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

        private Button getButtonByDateTime(DateTime dateTime)
        {
            int day = NUM_OF_DAYS - dateTime.Day;
            int hour = dateTime.Hour * 2;
            if (dateTime.Minute >= 30)
                hour++;
            Button button = ScheduleGrid.FindName("button_" + hour + "_" + day) as Button;
            return button;
        }
        private Boolean getStatusByDateTime(DateTime dateTime)
        {
            int day = NUM_OF_DAYS - dateTime.Day;
            int hour = dateTime.Hour * 2;
            if (dateTime.Minute >= 30)
                hour++;
            return tableOfHours[hour, day];
        }
        private Boolean isBlockNow()
        {
            return getStatusByDateTime(DateTime.Now);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tableOfHours = new bool[NUM_OF_DAYS, NUM_OF_HOURS];
            FromArrayOfBoolToButton(tableOfHours);
        }
    }
}
