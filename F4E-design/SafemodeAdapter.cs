﻿using Microsoft.Win32;
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
        public static void AddToSafeMode()
        {
            new Thread(() =>
            {
                AddToMinimalSafeMode();
                AddToWithNetworkSafeMode();
            }).Start();
        }

        public static void RemoveFromSafeMode()
        {
            DeleteFromMinimalSafeMode();
            DeleteFromWithNetworkSafeMode();
        }

        private static void AddToMinimalSafeMode()
        {
            try
            {
                RegistryKey key;
                key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\", true);
                key.CreateSubKey("GUIAdapter");
                key.Close();

                RegistryKey key2;
                key2 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\GUIAdapter", true);
                key2.SetValue("", "Service", RegistryValueKind.String);
                key2.Close();
            }
            catch(Exception e)
            {
                CustomMessageBox.ShowDialog(null, e.Message, @"\SafeBoot\Minimal\", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }

        private static void AddToWithNetworkSafeMode()
        {
            try
            {
                RegistryKey key;
                key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\", true);
                key.CreateSubKey("GUIAdapter");
                key.Close();

                RegistryKey key2;
                key2 = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\GUIAdapter", true);
                key2.SetValue("", "Service", RegistryValueKind.String);
                key2.Close();
            }
            catch (Exception e)
            {
                CustomMessageBox.ShowDialog(null, e.Message, @"\SafeBoot\Network\", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
            }
        }
        private static void DeleteFromMinimalSafeMode()
        {
            RegistryKey key;
            key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\", true);
            key.DeleteSubKey("GUIAdapter");
            key.Close();
        }

        private static void DeleteFromWithNetworkSafeMode()
        {
            RegistryKey key;
            key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot\Network\", true);
            key.DeleteSubKey("GUIAdapter");
            key.Close();
        }
    }
}
