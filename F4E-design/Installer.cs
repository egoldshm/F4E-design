using F4E_design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace F4E_GUI
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private Boolean installingProcess = true;

       

        private void On_Committed(object sender, InstallEventArgs e)
        {
            if (installingProcess)
            {
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                {
                    // create and show the window
                    System.Windows.Forms.MessageBox.Show("ההתקנה הסתיימה, מיד תועבר למסך ההגדרה הראשוני", "לוקחים אחריות", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    App.StartAgainAsAdmin();


                    // start the Dispatcher processing  
                    System.Windows.Threading.Dispatcher.Run();
                }));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();


            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                //Prevent uninstalling while the app is on
                if (!App.NoOtherProcessOpen())
                {
                    System.Windows.Forms.MessageBox.Show("לפני הסרת התקנה יש לסגור את המערכת. סגור את המערכת ולאחר מכן נסה שנית.");
                    throw new ApplicationException("לא ניתן להסיר את התוכנה בזמן שאחד המופעים שלה פתוח");
                }

                installingProcess = false;

                //ask for password
                UninstallPasswordForm uninstallPasswordForm = new UninstallPasswordForm();
                DialogResult dialogResult = uninstallPasswordForm.ShowDialog();
                uninstallPasswordForm.BringToFront();

                if (dialogResult == DialogResult.OK)
                {
                    string gettedPass = uninstallPasswordForm.enteredPassword;
                    if (gettedPass == "1234")
                    {

                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("הסיסמה שגויה, התראה נשלחה למנהל המערכת", "לוקחים אחריות", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        throw new ApplicationException("לא ניתן להסיר באמצעות סיסמה שגויה");
                    }
                }

                System.Windows.Forms.MessageBox.Show("sdfghjkl.");
                // start the Dispatcher processing  
                System.Windows.Threading.Dispatcher.Run();
            }));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            //System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
            //   

            //});     
        }
    }
}
