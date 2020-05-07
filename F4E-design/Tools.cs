using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace F4E_design
{
    static class Tools
    {
        public static bool CheckURLValid(this string source)
        {
            if (source == null) return false;
            return Regex.IsMatch(source, @"(http|https)://(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,3})$") || Regex.IsMatch(source, @"(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,3})$");
        }
    }
}
