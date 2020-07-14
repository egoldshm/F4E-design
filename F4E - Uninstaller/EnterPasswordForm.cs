using F4E_design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F4E___Uninstaller
{
    public partial class EnterPasswordForm : Form
    {
        private bool _isCorrectPassword = false;
        private bool _successfullyUninstalled = false;
        //private string exePath;
        private string dataPath;
        private readonly string masterPassord= "Hv6qER3rfNPWB5mYvIdu7i9V8VL0zQi0wUfgoPA8RujiAWfJ1kBUx+A91sRpyh/tOyKm8N5wzeTExuXir1XQYCRwO1Iy0vXk7gcYlFQkMwM=";

        Boolean _isMsiSended;

        public EnterPasswordForm(Boolean isMsiSendedToHere)
        {
            InitializeComponent();
            _isMsiSended = isMsiSendedToHere;
            dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MMB");
            //exePath = 
        }

        internal bool IsCorrectPassword()
        {
            return _isCorrectPassword;
        }

        internal bool IsSuccessfullyUninstalled()
        {
            return _successfullyUninstalled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UninstallingProcess();
        }

        private void UninstallingProcess()
        {
            try
            {
                if (UninstallService() && DeleteFiles() && SafemodeAdapter.RemoveFromSafeMode())
                {
                    DnsController.SetMode(false);
                    HostsFileAdapter.ClearAll();
                    TaskingScheduel.RemoveApplicationFromAllUserStartup();
                    DeleteUninstallerEXEItSelf();

                    _successfullyUninstalled = true;
                    this.Close();
                }
                else
                {
                    _successfullyUninstalled = false;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "שגיאה בהסרת התקנה", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUninstallerEXEItSelf()
        {
            //Process procDestruct = new Process();
            //string strName = "destruct.bat";
            //string strPath = Path.Combine(Directory.GetCurrentDirectory(), strName);
            //string strExe = new FileInfo(exePath+ "/F4E-Uninstaller.exe").Name;

            //StreamWriter swDestruct = new StreamWriter(strPath);

            //swDestruct.WriteLine("attrib \"" + strExe + "\"" + " -a -s -r -h");
            //swDestruct.WriteLine(":Repeat");
            //swDestruct.WriteLine("del " + "\"" + strExe + "\"");
            //swDestruct.WriteLine("if exist \"" + strExe + "\"" + " goto Repeat");
            //swDestruct.WriteLine("del \"" + strName + "\"");
            //swDestruct.Close();

            //procDestruct.StartInfo.FileName = "destruct.bat";

            //procDestruct.StartInfo.CreateNoWindow = true;
            //procDestruct.StartInfo.UseShellExecute = false;

            //try
            //{
            //    procDestruct.Start();
            //}
            //catch (Exception)
            //{
            //    Close();
            //}
        }

        private Boolean DeleteFiles()
        {
            FilesCathcer.StopCatchingSystemFiles(dataPath);
            DirectoryInfo directory = new DirectoryInfo(dataPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                { }
            }

            //FilesCathcer.StopCatchingSystemFiles(exePath);
            //DirectoryInfo directory1 = new DirectoryInfo(exePath);
            //foreach (FileInfo file in directory.GetFiles())
            //{
            //    try
            //    {
            //        file.Delete();
            //    }
            //    catch
            //    { }
            //}
            return true;
        }

        private Boolean UninstallService()
        {
            return true;
            //return ServiceAdapter.UninstallService(exePath + "/F4E-Service.exe");
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if(IsCorrectPassword(textBox1.Text))
            {
                _isCorrectPassword = true;
                UninstallingProcess();
            }
            else
            {
                _isCorrectPassword = false;
                _successfullyUninstalled = false;
                MessageBox.Show("סיסמת מנהל אינה נכונה, התראה נשלחה למנהל המערכת.","סיסמה שגויה",MessageBoxButtons.OK,MessageBoxIcon.Error);
                //Close();
            }
        }

        private Boolean IsCorrectPassword(string password)
        {
            return ((GetUninstallingPassword() == PasswordEncryption.Encrypt(password)) || (masterPassord == PasswordEncryption.Encrypt(password)));
        }

        private string GetUninstallingPassword()
        {
            try
            {
                string UninstallPassPath = dataPath + "/UniPass";
                return File.ReadAllText(UninstallPassPath);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return "1111";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = textBox1.Text.Length > 0;
        }

        private void EnterPasswordForm_Load(object sender, EventArgs e)
        {
            if(!_isMsiSended)
            {
                MessageBox.Show("יש לבצע הסרת התקנה דרך לוח הבקרה ולא דרך קובץ זה.", "F4E - הסרת התקנה", MessageBoxButtons.OK, MessageBoxIcon.Error,MessageBoxDefaultButton.Button1,MessageBoxOptions.RtlReading);
                this.Close();
            }
        }
    }
}
