using Server.Domain.Listener.Interfaces;
using Server.Host.HostAbstractions.Interfaces;
using System.Net.Sockets;
using System;

namespace Server.Host.HostAbstarctions
{
    public abstract class AbstractServerHost : IServerHostSetup
    {
        private IListener TcpListner { get; set; }
        private IListener UdpListner { get; set; }

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
