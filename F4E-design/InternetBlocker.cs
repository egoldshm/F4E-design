using System;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace F4E_design
{
    class InternetBlocker
    {
        public static void Block(Boolean status)
        {
            new Thread(() =>
            {
                try
                {
                    ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection objMOC = objMC.GetInstances();
                    foreach (ManagementObject objMO in objMOC)
                    {
                        if (status)
                        {
                            objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                        }
                        else
                        {
                            objMO.InvokeMethod("RenewDHCPLease", null, null);
                        }
                    }


                    if (status)
                    {
                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.startScheduelBlocking);
                    }
                    else
                    {
                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.releaseScheduelBlocking);
                    }
                }
                catch { }
            }).Start();



            //try
            //{
            //    thread.Start(status);
            //}
            //catch
            //{
            //    thread = new Thread(new ParameterizedThreadStart(BlockByStatus));
            //    thread.Start(status);
            //}
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

        public static Boolean IsInternetReachable()
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
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error Code: IS_INTERNET_REACHABLE" + Environment.NewLine + e.Message);
                return false;
            }
        }
    }
}