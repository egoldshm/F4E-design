using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace F4E_design
{
    class DnsController
    {
        private static readonly string PREFERRED_SAFE_DNS = "185.228.168.168";
        private static readonly string ALTERNATE_SAFE_DNS = "185.228.169.168";

        public static void SetMode(Boolean safeServer)
        {
            if (safeServer)
            {
                ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.setSafeDns);
            }
            else
            {
                ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.setDHCPDns);
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
