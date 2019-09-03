using Server.Domain.Listener.Interfaces;
using Server.Host.HostAbstractions.Interfaces;
using System.Net.Sockets;
using System;

namespace Server.Host.HostAbstarctions
{
    public abstract class AbstractServerHost : IServerHostSetup
    {
        protected IListener TcpListner { get; private set; }
        protected IListener UdpListner { get; private set; }

        public virtual void Run()
        {
        }

        public void SetListner(IListener listener)
        {
            switch(listener.ProtocolType)
            {
                case ProtocolType.Tcp:
                    TcpListner = listener;
                    break;
                case ProtocolType.Udp:
                    UdpListner = listener;
                    break;
                default:
                    throw new ArgumentException("Unsupported protocol type for Host Listner");
            }
        }
    }
}
