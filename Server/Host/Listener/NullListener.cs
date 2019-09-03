using Microsoft.Extensions.Logging;
using Server.Domain.Listener.Interfaces;
using System.Net.Sockets;

namespace Server.Host.Listener
{
    public class NullListener : IListener
    {
        public ProtocolType ProtocolType { get; }
        private ILogger<NullListener> Logger { get; }

        public NullListener(ProtocolType protocolType, ILogger<NullListener> logger)
        {
            ProtocolType = protocolType;
            Logger = logger;
        }

        public void Listen() => Logger.LogWarning("Listener for {0} protocol is unavailable.", (ProtocolType.Tcp == ProtocolType) ? "TCP" : "UDP");
    }
}
