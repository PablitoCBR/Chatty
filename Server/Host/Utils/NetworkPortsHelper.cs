using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Server.Host.Utils
{
    public class NetworkPortsHelper
    {
        private static readonly Lazy<NetworkPortsHelper> _instance = new Lazy<NetworkPortsHelper>(() => new NetworkPortsHelper());
        public static NetworkPortsHelper Instance { get => _instance.Value; }

        public bool IsTcpPortAvailable(ushort port)
             => !IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Any(x => x.LocalEndPoint.Port == port)
            && !IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(x => x.Port == port);

        public bool IsUdpPortAvailable(ushort port)
            => !IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(x => x.Port == port);

        public ushort GetAvaliblePort(ushort startingPort)
        {
            for (ushort port = startingPort; port < UInt16.MaxValue; port++)
                if (IsTcpPortAvailable(port) && IsUdpPortAvailable(port))
                    return port;
            return 0;
        }

        public ushort GetAvalibleUdpPort(ushort startingPort)
        {
            for (ushort port = startingPort; port < UInt16.MaxValue; port++)
                if (IsUdpPortAvailable(port)) return port;
            return 0;
        }

        public ushort GetAvalibleTcpPort(ushort startingPort)
        {
            for(ushort port = startingPort; port < UInt16.MaxValue; port++)
                if (IsTcpPortAvailable(port)) return port;
            return 0;
        }
    }
}
