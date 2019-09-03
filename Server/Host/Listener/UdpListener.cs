using System.Net.Sockets;
using Server.Domain.Listener.Interfaces;

namespace Server.Host.Listener
{
    public class UdpListener : IListener
    {
        public ProtocolType ProtocolType { get => ProtocolType.Udp;  }

        public void Listen()
        {
            throw new System.NotImplementedException();
        }
    }
}
