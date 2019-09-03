using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Domain.Listener.Interfaces;

namespace Server.Host.Listener
{
    public class UdpListener : IListener
    {
        public ProtocolType ProtocolType { get => ProtocolType.Udp;  }

        public async void Listen()
        {
            throw new System.NotImplementedException();
        }
    }
}
