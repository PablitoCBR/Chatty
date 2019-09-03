using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Domain.Listener.Interfaces
{
    public interface IListener
    {
        ProtocolType ProtocolType { get; }
        void Listen();
    }
}
