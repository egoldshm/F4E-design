using System;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace F4E___Service
{
    class InternetBlocker
    {
        static Thread thread = new Thread(new ParameterizedThreadStart(BlockByStatus));
        private static Boolean blockedStatus = false;
        public static void Block(Boolean status)
        {
            try
            {
                thread.Start(status);
            }
            catch
            {
                thread = new Thread(new ParameterizedThreadStart(BlockByStatus));
                thread.Start(status);
            }
        }

        private static void BlockByStatus(object data)
        {
            blockedStatus = (Boolean)data;
            try
            {
                ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection objMOC = objMC.GetInstances();
                foreach (ManagementObject objMO in objMOC)
                {
                    if (blockedStatus)
                    {
                        objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                    }
                    else
                    {
                        objMO.InvokeMethod("RenewDHCPLease", null, null);
                    }
                }
            }
            catch { }
        }

        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInternetAvailable()
        {
            try
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    if (properties.DnsSuffix != "" || properties.GatewayAddresses.Count > 0)
                    {
                        return CheckForInternetConnection();
                    }
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static Boolean GetBlockStatus()
        {
            return blockedStatus;
        }
    }
}