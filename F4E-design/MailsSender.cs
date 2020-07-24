using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using F4E_design;
using System.Windows.Forms;
using System.Threading;

namespace F4E_GUI
{
    class MailsSender
    {

        private static string SenderPassword= "buKAhtzMeexBBGbKYlSEMl9DOLB5RXm7utxMclom1Yw=";
        private static readonly string SenderAddress = "f4e@mmb.org.il";

        public static void SendCustomMail(string toAddress, string subject, string body)
        {
            ReloadPassword();

            var fromMailAddress = new MailAddress(SenderAddress, "F4E by MMB");
            var toMailAddress = new MailAddress(toAddress, "");
            string fromPassword = PasswordEncryption.Decrypt(SenderPassword);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMailAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(SenderAddress, toAddress)
            {
                Subject = subject,
                Body = body,
            })
            {
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }

        private static string ReplaceCustomTagsToData(string original, DateTime date)
        {
            try
            {
                string formatted;
                formatted = original.Replace("%name%", FilteringSystem.GetCurrentFilteringSettings().GetAdminName());
                formatted = formatted.Replace("%date%", date.ToShortDateString() + " בשעה: " + date.ToShortTimeString());
                formatted = formatted.Replace("%pc_name%", FilteringSystem.GetCurrentFilteringSettings().GetComputerName());
                formatted = formatted.Replace("%password%", PasswordEncryption.Decrypt(FilteringSystem.GetCurrentFilteringSettings().GetAdminPassword()));
                return formatted;
            }
            catch
            {
                return null;
            }
        }

        private static void ReloadPassword()
        {
            try
            {
                SenderPassword = Tools.GetTextFromUri("http://f4e.mmb.org.il/data/cre587");
            }
            catch
            {
                SenderPassword = "eBen3TJ60aye49KIqqXKWXBnGqNeZ0UIB4uck0sQsWM=";
            }
        }

        public static void SendWrongPasswordAlert()
        {
            string subject, body;
            subject = "F4E - התראה על שימוש בסיסמה שגויה";
            new Thread(() =>
            {
                Boolean sended = false;
                DateTime Time_Now = DateTime.Now;
                while (!sended)
                {
                    try
                    {
                        body = ReplaceCustomTagsToData(Tools.GetTextFromUri("http://f4e.mmb.org.il/mails-templets/wrong_password"), Time_Now);
                        SendCustomMail(FilteringSystem.GetCurrentFilteringSettings().GetAdminMail(), subject, body);
                        sended = true;
                    }
                    catch
                    {
                        Thread.Sleep(120000);
                    }
                }
            }).Start();
        }
        public static void SendUnusualActivityAlert()
        {
            string subject, body;
            subject = "F4E - התראה על פעילות חשודה";
            new Thread(() =>
            {
                Boolean sended = false;
                DateTime Time_Now = DateTime.Now;
                while (!sended)
                {
                    try
                    {
                        body = ReplaceCustomTagsToData(Tools.GetTextFromUri("http://f4e.mmb.org.il/mails-templets/unusual_activity"), Time_Now);
                        SendCustomMail(FilteringSystem.GetCurrentFilteringSettings().GetAdminMail(), subject, body);
                        sended = true;
                    }
                    catch
                    {
                        Thread.Sleep(120000);
                    }
                }
            }).Start();
        }

        internal static void SendPasswordReminderMail()
        {
            string subject, body;           
            new Thread(() =>
            {
                subject = "F4E - תזכורת לסיסמה שנשכחה";
                Boolean sended = false;
                DateTime Time_Now = DateTime.Now;
                while (!sended)
                {
                    try
                    {
                        body = ReplaceCustomTagsToData(Tools.GetTextFromUri("http://f4e.mmb.org.il/mails-templets/password_reminder"), Time_Now);
                        SendCustomMail(FilteringSystem.GetCurrentFilteringSettings().GetAdminMail(), subject, body);
                        sended = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        Thread.Sleep(120000);
                    }
                }
            }).Start();
        }
    }
}