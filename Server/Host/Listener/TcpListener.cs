using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Domain.Listener.Interfaces;

namespace Server.Host.Listener
{
    public class TcpListener : IListener
    {
        private ILogger Logger { get; }
        private Socket Listener { get; }
        private IPEndPoint IPEndPoint { get; }
        private uint BufferSize { get; }
        private int ConnectionsQueueLength { get; }
        private string EndOfStreamMarker { get; }
        public ProtocolType ProtocolType { get => ProtocolType.Tcp; }

        public TcpListener(IPAddress  ipAddress, ushort port, uint bufferSize, int pendingConnectionQueueLength, string endOfStreamMarker, ILogger<TcpListener> logger)
        {
            Logger = logger;
            IPEndPoint = new IPEndPoint(ipAddress, port);
            Listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            BufferSize = bufferSize;
            ConnectionsQueueLength = pendingConnectionQueueLength;
            EndOfStreamMarker = endOfStreamMarker;

            Listener.Bind(IPEndPoint);
        }

        public void Listen()
        {
            Logger.LogInformation("Staring listening with TCP on port: {0}", IPEndPoint.Port);
            Listener.Listen(ConnectionsQueueLength);

            while(true)
            {
                Socket clientSocket = Listener.Accept();
                byte[] buffer = new byte[BufferSize];
                string message = String.Empty;

                do
                {
                    clientSocket.Receive(buffer);
                    message += Encoding.ASCII.GetString(buffer);
                } while (!message.Contains(EndOfStreamMarker));
                Task.Run(() => HandleMessage(message, clientSocket));
            }
        }

        private Task HandleMessage(string receivedData, Socket clientSocket)
        {
            string message = receivedData.Substring(0, receivedData.IndexOf(EndOfStreamMarker));
            Logger.LogInformation("Recived message: {0}", message);
            Thread.Sleep(10000);
            clientSocket.Send(Encoding.ASCII.GetBytes(message));
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            return Task.CompletedTask;
        }
    }
}
