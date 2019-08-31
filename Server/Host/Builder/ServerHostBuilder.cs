using System;
using Microsoft.Extensions.Logging;
using Server.Host.Builder.Interfaces;
using Server.Host.HostAbstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Server.Utils;
using Server.Host.Builder.Exceptions;
using Server.Host.Utils;
using System.Net.Sockets;

namespace Server.Host.Builder
{
    public class ServerHostBuilder : IServerHostBuilder
    {
        public IServiceProvider ServiceProvider { get; }
        private ILogger<IServerHostBuilder> Logger { get; }
        public BuilderSettings Settings { get; set; }

        public ServerHostBuilder(IServiceProvider serviceProvider, ILogger<IServerHostBuilder> logger)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
        }


        public IServerHost Build()
        {
            IServerHostSetup serverHost = ServiceProvider.GetService<IServerHostSetup>();
            Common.TryCatchAction<ServerHostBuilderPortException>(VerifyPort, ex => {
                Logger.LogWarning("Host Builder will search for first avilable port");
                Settings.Port = NetworkPortsHelper.Instance.GetAvaliblePort(Settings.Port);
                Logger.LogWarning("Host Builder set up new port: {0}", Settings.Port);
            }, Logger);
        }

        private void VerifyPort()
        {
            if (!NetworkPortsHelper.Instance.IsTcpPortAvailable(Settings.Port))
                throw new ServerHostBuilderPortException("Port is not available!", Settings.Port, ProtocolType.Tcp);
            if(!NetworkPortsHelper.Instance.IsTcpPortAvailable(Settings.Port))
                throw new ServerHostBuilderPortException("Port is not available!", Settings.Port, ProtocolType.Udp);
        }
    }
}
