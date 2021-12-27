using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Encrychat
{
    public static class Settings
    {
        public const int Port = 50495;
        public static readonly IPAddress LocalAddress = GetLocalIPv4(NetworkInterfaceType.Ethernet);
        public const int KeySize = 2048;
        public const int SeparatorLinesSize = 23;
        public const int MessageDataBufferSize = 64;
        
        private static IPAddress GetLocalIPv4(NetworkInterfaceType type)
        {
            var result = string.Empty;
            
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == type && 
                        networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            result = ip.Address.ToString();
                        }
                    }
                }
            }
            
            return IPAddress.Parse(result);
        }
    }
}