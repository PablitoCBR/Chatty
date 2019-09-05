using Server.Domain.Listener.Interfaces;
using Server.Host.HostAbstractions.Interfaces;
using System.Net.Sockets;
using System;
using Microsoft.Extensions.Logging;

namespace Server.Host.HostAbstarctions
{
    public abstract class AbstractServerHost : IServerHostSetup
    {
        protected IListener TcpListener { get; private set; }
        protected IListener UdpListener { get; private set; }
        protected ILogger<IServerHost> Logger { get; }

        public AbstractServerHost(ILogger<IServerHost> logger)
        {
            Logger = logger;
        }


        public void SetListner(IListener listener)
        {
            switch (listener.ProtocolType)
            {
                case ProtocolType.Tcp:
                    TcpListener = listener;
                    break;
                case ProtocolType.Udp:
                    UdpListener = listener;
                    break;
                default:
                    throw new ArgumentException("Unsupported protocol type for Host Listner");
            }
        }

        public abstract void Dispose();
        public abstract void Run();

        public void StopListeners()
        {
            if (TcpListener != null)
                if (TcpListener.IsListening)
                    TcpListener.Stop();
                else Logger.LogInformation("TCP listener already stopped");
            else Logger.LogWarning("TCP listener is not set!");

            if (UdpListener != null)
                if (UdpListener.IsListening)
                    UdpListener.Stop();
                else Logger.LogInformation("UDP listener already stopped");
            else Logger.LogWarning("UDP listener is not set!");
        }

        public void StartListeners()
        {
            if (TcpListener != null)
                if (TcpListener.IsListening)
                    Logger.LogInformation("TCP listener already listening");
                else TcpListener.Listen();
            else Logger.LogWarning("TCP listener is not set!");

            if (UdpListener != null)
                if (UdpListener.IsListening)
                    Logger.LogInformation("UDP listener already listening");
                else UdpListener.Listen();
            else Logger.LogWarning("UDP listener is not set!");
        }
    }
}
