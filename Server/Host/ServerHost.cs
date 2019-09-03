using Server.Host.HostAbstarctions;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Host
{
    public class ServerHost : AbstractServerHost
    {
        private Thread _tcpListenerThread;
        private Thread _udpListenerThread;

        public override void Run()
        {
            if (_tcpListenerThread == null)
            {
                _tcpListenerThread = new Thread(TcpListner.Listen);
                _tcpListenerThread.Start();
            }
            if (_udpListenerThread == null)
            {
                _udpListenerThread = new Thread(UdpListner.Listen);
                _udpListenerThread.Start();
            }
        }
    }
}
