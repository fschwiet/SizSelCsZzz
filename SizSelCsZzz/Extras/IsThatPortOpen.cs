using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace SizSelCsZzz.Extras
{
    public class IsThatPortOpen
    {
        // http://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available
        public static bool HasExistingTCPListener(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
