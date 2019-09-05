using Microsoft.Extensions.Logging;
using Server.Host.HostAbstarctions;
using Server.Host.HostAbstractions.Interfaces;
using Server.Utils;
using System;
using System.Threading;

namespace Server.Host
{
    public class ServerHost : AbstractServerHost
    {
        private Thread _tcpListenerThread;
        private Thread _udpListenerThread;  

        public ServerHost(ILogger<IServerHost> logger) : base(logger) { }

        public override void Run()
        {
            if (_tcpListenerThread != null && _tcpListenerThread.IsAlive)
            {
                TcpListener.Stop();
                TcpListener?.Dispose();
                Common.TryCatchAction<ThreadAbortException>(() => _tcpListenerThread.Abort(), (ex) =>
                {
                    Logger.LogWarning(ex, "Existing TCP listener thread was aborted.");
                    _tcpListenerThread = null;
                });
            }
            
            _tcpListenerThread = new Thread(() => TcpListener.Listen());
            _tcpListenerThread.Start();

            if (_udpListenerThread != null && _udpListenerThread.IsAlive)
            {
                UdpListener.Stop();
                UdpListener?.Dispose();
                Common.TryCatchAction<ThreadAbortException>(() => _udpListenerThread.Abort(), (ex) =>
                {
                    Logger.LogWarning(ex, "Existing UDP listener thread was aborted.");
                    _udpListenerThread = null;
                });
            }
            
            _udpListenerThread = new Thread(() => UdpListener.Listen());
            _udpListenerThread.Start();
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            TcpListener.Stop();
            UdpListener.Stop();

            if (dispose)
            {
                TcpListener.Dispose();
                UdpListener.Dispose();
            }

            Common.TryCatchAction<ThreadAbortException>(() => _tcpListenerThread.Abort(), (ex) =>
            {
                Logger.LogWarning(ex, "Existing TCP listener thread was aborted.");
                _tcpListenerThread = null;
            });

            Common.TryCatchAction<ThreadAbortException>(() => _udpListenerThread.Abort(), (ex) =>
            {
                Logger.LogWarning(ex, "Existing UDP listener thread was aborted.");
                _udpListenerThread = null;
            });
        }
    }
}
