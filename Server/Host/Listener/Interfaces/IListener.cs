using System;
using System.Net.Sockets;

namespace Server.Domain.Listener.Interfaces
{
    public interface IListener : IDisposable
    {
        ProtocolType ProtocolType { get; }
        bool IsListening { get; }
        void Listen();
        void Stop();
    }
}
