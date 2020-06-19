using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F4E_GUI
{
    public partial class UninstallPasswordForm : Form
    {
        public UninstallPasswordForm()
        {
            InitializeComponent();
        }

        public string enteredPassword;

        private void button1_Click(object sender, EventArgs e)
        {
            enteredPassword = textBox1.Text;
        }
    }
}
