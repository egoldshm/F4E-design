﻿using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;

namespace F4E___Service
{
    class DnsController
    {
        private static readonly string PREFERRED_SAFE_DNS = "185.228.168.168";
        private static readonly string ALTERNATE_SAFE_DNS = "185.228.169.168";

        public static void SetMode(Boolean safeServer)
        {
            if (safeServer)
            {
                SetSafeDNS();
            }
            else
            {
                SetDHCP_DNS();
            }
        }
        public static void SetSafeDNS()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                SetDNS(adapter, PREFERRED_SAFE_DNS, ALTERNATE_SAFE_DNS);
            }
        }

        public static void SetDHCP_DNS()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                UnsetDNS(adapter);
            }
        }


        private static void SetDNS(NetworkInterface adapter, string PreferredDNS, string AlternateDNS)
        {
            string[] Dns = { PreferredDNS, AlternateDNS };
            var CurrentInterface = adapter;
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = Dns;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }
        private static void UnsetDNS(NetworkInterface adapter)
        {
            var CurrentInterface = adapter;
            if (CurrentInterface == null) return;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].ToString().Contains(CurrentInterface.Description))
                    {
                        ManagementBaseObject objdns = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        if (objdns != null)
                        {
                            objdns["DNSServerSearchOrder"] = null;
                            objMO.InvokeMethod("SetDNSServerSearchOrder", objdns, null);
                        }
                    }
                }
            }
        }

        public static Boolean IsSafe(Boolean showDebugMessage)
        {
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        if (networkInterface.OperationalStatus == OperationalStatus.Up)
                        {
                            IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                            IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                            foreach (IPAddress dnsAddress in dnsAddresses)
                            {
                                if (dnsAddress.MapToIPv4().ToString() != PREFERRED_SAFE_DNS && dnsAddress.MapToIPv4().ToString() != ALTERNATE_SAFE_DNS)
                                {
                                    if (showDebugMessage)
                                        System.Windows.Forms.MessageBox.Show(networkInterface.Name + "   " + networkInterface.NetworkInterfaceType + "     " + dnsAddress.ToString());
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error Code: IS_SAFE_SERVER" + Environment.NewLine + e.Message);
                return false;
            }
        }
    }
}
