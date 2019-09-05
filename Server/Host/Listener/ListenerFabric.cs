using Server.Domain.Listener.Interfaces;
using Server.Host.Builder.Exceptions;
using Server.Host.Listener.Interfaces;
using Server.Host.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server.Host.Listener
{
    public class ListenerFabric : IListenerFabric
    {
        private IPAddress IPAddress { get; }
        private IServiceProvider ServiceProvider { get; }

        public ListenerFabric(IServiceProvider serviceProvider)
        {
            IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            ServiceProvider = serviceProvider;
        }

        /// <exception cref="ServerHostBuilderPortException">Exception is throw if port is unavailable</exception>
        public IListener CreateTCP(ushort port, int pendingConnectionQueue, int bufferSize, string endOfStreamMarker)
        {
            if (!NetworkPortsHelper.Instance.IsTcpPortAvailable(port))
                throw new ServerHostBuilderPortException("Port is not available!", port, ProtocolType.Tcp);

            return new TcpListener(IPAddress, port, bufferSize, pendingConnectionQueue, endOfStreamMarker, ServiceProvider.GetService<ILogger<TcpListener>>());
        }

        /// <exception cref="ServerHostBuilderPortException">Exception is throw if port is unavailable</exception>
        public IListener CreateUDP(ushort port, int bufferSize)
        {
            if (!NetworkPortsHelper.Instance.IsUdpPortAvailable(port))
                throw new ServerHostBuilderPortException("Port is not available!", port, ProtocolType.Udp);

            return new NullListener(ProtocolType.Udp, ServiceProvider.GetService<ILogger<NullListener>>());
        }
    }
}
