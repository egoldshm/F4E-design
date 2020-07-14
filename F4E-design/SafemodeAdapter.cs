using F4E_design;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace F4E_GUI
{
    class SafeModeAdapter
    {
        private static String GetRegistryKeyValue(RegistryHive baseKey, string subKey, string value)
        {
            RegistryKey baseRegistryKey = RegistryKey.OpenBaseKey(baseKey, RegistryView.Registry64);
            RegistryKey subRegistryKey = baseRegistryKey.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadSubTree);
            if (subRegistryKey != null)
            {
                object value64 = subRegistryKey.GetValue(value);
                if (value64 != null)
                {
                    baseRegistryKey.Close();
                    subRegistryKey.Close();
                    return (string)value64;
                }
                subRegistryKey.Close();
            }
            baseRegistryKey.Close();
            return null;
        }

        public static Boolean IsRunInSafeMode()
        {
            string minimalSafeMode = GetRegistryKeyValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\GUIAdapter", "");
            string networkSafeMode = GetRegistryKeyValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\GUIAdapter", "");
            if (minimalSafeMode != null && networkSafeMode != null)
            {
                if (minimalSafeMode == "Service" && networkSafeMode == "Service")
                {
                    return true;
                }
                return false;
            }
            else
            {
                ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.addToSafeMode);
                return false;
            }
        }
    }
}
