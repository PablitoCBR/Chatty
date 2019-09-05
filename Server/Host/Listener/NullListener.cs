using Microsoft.Extensions.Logging;
using Server.Domain.Listener.Interfaces;
using System;
using System.Net.Sockets;

namespace Server.Host.Listener
{
    public class NullListener : IListener
    {
        public ProtocolType ProtocolType { get; }
        private ILogger<NullListener> Logger { get; }

        public bool IsListening => false;

        public NullListener(ProtocolType protocolType, ILogger<NullListener> logger)
        {
            ProtocolType = protocolType;
            Logger = logger;
        }

        public void Listen() => Logger.LogWarning("Listen action called: Listener for {0} protocol is unavailable.", (ProtocolType.Tcp == ProtocolType) ? "TCP" : "UDP");

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Stop() => Logger.LogWarning("Stop action called: Listener for {0} protocol is unavailable.", (ProtocolType.Tcp == ProtocolType) ? "TCP" : "UDP");
    }
}
