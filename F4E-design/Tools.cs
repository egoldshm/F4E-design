using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
