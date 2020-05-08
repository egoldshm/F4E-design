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
using System.Windows.Shapes;

namespace F4E_design
{
    /// <summary>
    /// Interaction logic for EnterAdminPasswordWindow.xaml
    /// </summary>
    public partial class EnterAdminPasswordWindow : Window
    {
        public EnterAdminPasswordWindow(Window OwnerWindow)
        {
            InitializeComponent();
            this.Owner = OwnerWindow;
            passwordTB.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (passwordTB.Password == "1234")
            {
                CustomMessageBox.ShowDialog(this, "ברוך הבא, ניב!", "סיסמה נכונה!", CustomMessageBox.CustomMessageBoxTypes.Success);
                this.Close();
            }
            else
            {
                CustomMessageBox.ShowDialog(this, "לצערנו, הקלדת את הסיסמה הלא נכונה", "סיסמה שגויה", CustomMessageBox.CustomMessageBoxTypes.Error);

            }
        }
    }
}
