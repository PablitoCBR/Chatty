using System;
using Microsoft.Extensions.Logging;
using Server.Host.Builder.Interfaces;
using Server.Host.HostAbstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Server.Utils;
using Server.Host.Builder.Exceptions;
using Server.Host.Utils;
using Server.Domain.Listener.Interfaces;
using Server.Host.Listener.Interfaces;

namespace Server.Host.Builder
{
    public class ServerHostBuilder : IServerHostBuilder
    {
        private IServiceProvider ServiceProvider { get; }
        private ILogger<IServerHostBuilder> Logger { get; }
        private IListenerFabric ListenerFabric { get; } 
        public BuilderSettings Settings { get;  set; }

        public ServerHostBuilder(IServiceProvider serviceProvider, ILogger<IServerHostBuilder> logger, IListenerFabric listenerFabric)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
            ListenerFabric = listenerFabric;
        }


        public IServerHost Build()
        {
            IServerHostSetup serverHost = ServiceProvider.GetService<IServerHostSetup>();
            BuildListeners(serverHost);
            return serverHost;
        }

        private void BuildListeners(IServerHostSetup serverHost)
        {
            IListener tcpListener = Common.TryCatchFunc<IListener, ServerHostBuilderPortException>(
                () => ListenerFabric.CreateTCP(Settings.Port, Settings.TcpPendingConnectionsQueueLength, Settings.TcpBufferSize, Settings.EndOfStreamMarker),
                HandlePortException, Logger);

            IListener udpListner = Common.TryCatchFunc<IListener, ServerHostBuilderPortException>(
                () => ListenerFabric.CreateUDP(Settings.Port, Settings.UdpBufferSize), 
                HandlePortException, Logger);

            serverHost.SetListner(tcpListener);
            serverHost.SetListner(udpListner);
        }

        private IListener HandlePortException(ServerHostBuilderPortException exception)
        {
            Logger.LogError(exception.ToString());
            SetFirstAvailablePort();
            if (exception.ProtocolType == System.Net.Sockets.ProtocolType.Tcp)
                return ListenerFabric.CreateTCP(Settings.Port, Settings.TcpPendingConnectionsQueueLength, Settings.TcpBufferSize, Settings.EndOfStreamMarker);
            else return ListenerFabric.CreateUDP(Settings.Port, Settings.UdpBufferSize);
        }

        private void SetFirstAvailablePort()
        {
            Logger.LogWarning("Host Builder will search for first avilable port");
            Settings.Port = NetworkPortsHelper.Instance.GetAvaliblePort(Settings.Port);
            Logger.LogWarning("Host Builder sets up new port: {0}", Settings.Port);
        }
    }
}
