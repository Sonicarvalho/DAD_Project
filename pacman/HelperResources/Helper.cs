using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pacman.HelperResources
{
    class Helper
    {
        
        private static string getMyIp()
        {   
             string[] ignoreHostname = { "DESKTOP-AMS9BIJ" };
            string hostname = Environment.MachineName;
            int pos = Array.IndexOf(ignoreHostname, hostname);
            if (pos > -1)
            {
                //There will be trouble if we dont ignore this hostname
                return "localhost";
            }
            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection)
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1)
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    return address.Address.ToString();
                }
            }
            return "localhost";
        }
    }
}
