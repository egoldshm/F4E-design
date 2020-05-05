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
        public static readonly Brush COLOR_OF_UNSELECTED_BUTTON = Brushes.White;

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
                Label LabelForDay = new Label();
                LabelForDay.SetValue(Grid.RowProperty, 0);
                LabelForDay.SetValue(Grid.ColumnProperty, j + 1);
                LabelForDay.Name = "day_button" + j;
                LabelForDay.BorderThickness = new Thickness(1);
                LabelForDay.HorizontalContentAlignment = HorizontalAlignment.Center;
                LabelForDay.VerticalContentAlignment = VerticalAlignment.Center;
                LabelForDay.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                LabelForDay.Background = (Brush)converter.ConvertFromString("#FFCBD8E6");
                LabelForDay.FontFamily = new FontFamily("Assistant Bold");
                LabelForDay.FontWeight = FontWeights.Bold;
                LabelForDay.FontSize = 13;
                LabelForDay.MouseMove += DayButton_MouseMove;
                LabelForDay.Content = days[j];
                ScheduleGrid.Children.Add(LabelForDay);
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

                //hourLabel.MouseLeftButtonDown += HourLabel_MouseLeftButtonDown;
                hourLabel.MouseMove += HourLabel_MouseMove;
                
                ScheduleGrid.Children.Add(hourLabel);
                for (int j = 0; j < 7; j++)
                {
                    Button newButton = new Button();
                    newButton.SetValue(Grid.RowProperty, i + 1);
                    newButton.SetValue(Grid.ColumnProperty, j + 1);
                    newButton.Name = "button_" + i + "_" + j;
                    this.RegisterName(newButton.Name, newButton);
                    newButton.BorderBrush = (Brush)converter.ConvertFromString("#FF0B52A7");
                    newButton.Background = Brushes.White;

                    newButton.MouseMove += clickOnHourButton;

                    ScheduleGrid.Children.Add(newButton);
                }

                
            }

        }

        private void DayButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Label buttonSender = sender as Label;
                int day = (int)buttonSender.GetValue(Grid.ColumnProperty) - 1;

                Brush colorToBrush = COLOR_OF_SELECTED_BUTTON;
                bool boolToInsert = true;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    colorToBrush = COLOR_OF_UNSELECTED_BUTTON;
                    boolToInsert = false;
                }

                for (int i = 0; i < 48; i++)
                {
                    Button button = ScheduleGrid.FindName("button_" + i + "_" + day) as Button;
                    button.Background = colorToBrush;
                    tableOfHours[day, i] = boolToInsert;

                }
            }
        }

        private void HourLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Label labelSender = sender as Label;
                int hour = (int)labelSender.GetValue(Grid.RowProperty) - 1;
                Brush colorToBrush = COLOR_OF_SELECTED_BUTTON;
                bool boolToInsert = true;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    colorToBrush = COLOR_OF_UNSELECTED_BUTTON;
                    boolToInsert = false;
                }
                for (int i = 0; i < 7; i++)
                {
                    Button button = ScheduleGrid.FindName("button_" + hour + "_" + i) as Button;
                    button.Background = colorToBrush;
                    tableOfHours[i, hour] = boolToInsert;

                }
            }
        }

      
        private void unchooseFullDay(object sender, MouseButtonEventArgs e)
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

        private void clickOnHourButton(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Button button = sender as Button;
                int day = 7 - (int)button.GetValue(Grid.ColumnProperty);
                int hour = (int)button.GetValue(Grid.RowProperty) - 1;
                if (tableOfHours[day, hour])
                {
                    button.Background = COLOR_OF_UNSELECTED_BUTTON;
                }
                else
                {
                    button.Background = COLOR_OF_SELECTED_BUTTON;
                }
                tableOfHours[day, hour] = !tableOfHours[day, hour];
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
