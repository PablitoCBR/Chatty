using System.Net.Sockets;

namespace Server.Domain.Listener.Interfaces
{
    public interface IListener
    {
        ProtocolType ProtocolType { get; }
        void Listen();
    }
}
