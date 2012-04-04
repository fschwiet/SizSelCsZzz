using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace SizSelCsZzz.Extras
{
    public class TCPUtil
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

        static public int GetAvailableTCPPort(int minValue, int maxValue)
        {
            var port = minValue;

            while(HasExistingTCPListener(port))
            {
                port++;

                if (port > maxValue)
                {
                    throw new Exception(String.Format("Couldn't find available port.. (checked {0}-{1}).", minValue, maxValue));
                }
            }
            return port;
        }
    }
}
