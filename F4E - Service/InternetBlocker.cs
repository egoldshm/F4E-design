using System;
using System.Management;
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

        public static bool IsInternetAvailable()
        {
            try
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
            catch
            {
                return false;
            }
        }

        private static bool InternetGetConnectedState(out int description, int v)
        {
            throw new NotImplementedException();
        }

        public static Boolean GetBlockStatus()
        {
            return blockedStatus;
        }
    }
}