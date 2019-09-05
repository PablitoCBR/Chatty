using System.Net.Sockets;
using System.Threading;
using Server.Domain.Listener.Interfaces;

namespace Server.Host.Listener
{
    public class UdpListener : IListener
    {
        public ProtocolType ProtocolType { get => ProtocolType.Udp;  }

        public bool IsListening => false;

        public void Dispose()
        {
            
        }

        private void Listen(CancellationToken cancellationToken)
        {
        }

        public void Listen()
        {
        }

        public void Stop()
        {
        }
    }
}
