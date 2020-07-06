using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace F4E_design
{
    class SafemodeAdapter
    {

        public static Boolean RemoveFromSafeMode()
        {
            try
            {

                RegistryKey networkSafeMode;
                networkSafeMode = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\GUIAdapter", true);


                DeleteFromMinimalSafeMode();
                DeleteFromWithNetworkSafeMode();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void DeleteFromMinimalSafeMode()
        {
            RegistryKey minimalSafeMode;
            minimalSafeMode = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal", true);
            if (minimalSafeMode.GetSubKeyNames().Contains("GUIAdapter"))
                minimalSafeMode.DeleteSubKey("GUIAdapter");
            minimalSafeMode.Close();
        }

        private static void DeleteFromWithNetworkSafeMode()
        {
            RegistryKey networkSafeMode;
            networkSafeMode = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network", true);
            if (networkSafeMode.GetSubKeyNames().Contains("GUIAdapter"))
                networkSafeMode.DeleteSubKey("GUIAdapter");
            networkSafeMode.Close();
        }
    }
}
