using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace F4E_design
{
    static class Tools
    {
        public static bool CheckURLValid(this string source)
        {
            if (source == null) return false;
            return Regex.IsMatch(source, @"(http|https)://(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,3})$") || Regex.IsMatch(source, @"(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,3})$");
        }

        public static bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        public static void KillProcessByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach(Process process in processes)
            {
                process.Kill();
            }
        }

        public static BitmapImage LoadBitmapFromResource(string pathInApplication, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
        }

        public static string FormatToShortDomainUri(string url)
        {
            url = url.ToLower().Replace("http://", "").Replace("https://", "");
            if (url.ToLower().IndexOf("www.") == 0)
                url = url.Substring(4);
            if (url[url.Length - 1] == '/')
                url = url.Substring(0, url.Length - 1);
            return url;
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static string GetTextFromUri(string uri)
        {
            try
            {
                WebClient client = new WebClient();
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                client.Headers.Add("Cache-Control", "no-cache");
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(uri);
            }
            catch
            {
                return null;
            }
        }
   
       
    }
}
