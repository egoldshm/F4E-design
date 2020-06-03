using F4E_design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace F4E_GUI
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private Boolean installingProcess = true;

        public Installer()
        {
            InitializeComponent();
            this.Committed += new InstallEventHandler(On_Committed);
        }


        private void On_Committed(object sender, InstallEventArgs e)
        {
            if (installingProcess)
            {
                //.Show("ההתקנה הסתיימה, לחץ למעבר למסך ההגדרה הראשוני", "לוקחים אחריות", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                App.StartAgainAsAdmin();
            }
        }
    }
}
