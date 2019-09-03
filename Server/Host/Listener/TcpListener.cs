using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Domain.Listener.Interfaces;
using Server.Utils;

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
                HandleIncomingDataAsync(clientSocket).ConfigureAwait(false);
            }
        }


        /// TEMP METHOD !!! Will be replaced by class handling recived data (extracting recepient and passing message)
        private Task HandleMessageAsync(string message, Socket clientSocket)
        {
            Logger.LogInformation("Recived message: {0}", message);
            clientSocket.Send(Encoding.ASCII.GetBytes(message));
            return Task.CompletedTask;
        }

        private async Task HandleIncomingDataAsync(Socket clientSocket)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                string messageReceived = await ReceiveDataAsync(clientSocket, cts.Token).ConfigureAwait(false);
                if(!String.IsNullOrEmpty(messageReceived))
                    await HandleMessageAsync(messageReceived, clientSocket).ConfigureAwait(false);
            }
            catch(AggregateException aggregateException)
            {
                foreach (Exception ex in aggregateException.InnerExceptions)
                    Logger.LogError(ex, "Exception occured while receiving data from {0}", ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
            }
            finally
            {
                await CloseSocketAsync(clientSocket).ConfigureAwait(false);
            }
        }

        private async Task<string> ReceiveDataAsync(Socket clientSocket, CancellationToken cancellationToken)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[BufferSize]);
            string message = String.Empty;
            do
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Logger.LogWarning("TCP connection canceled after 5s timeout {0}Remote IPv6: {1}", Environment.NewLine, ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
                        return await Task.FromResult(String.Empty);
                    }
                    await clientSocket.ReceiveAsync(buffer, SocketFlags.None).ConfigureAwait(false);
                    message += Encoding.ASCII.GetString(buffer);
                }
                catch(AggregateException ex)
                {
                    Logger.LogError("Data receiving from {0} has to stopped due to exception", ((IPEndPoint)clientSocket?.RemoteEndPoint)?.Address.ToString());
                    throw ex;
                }
            } while (!message.Contains(EndOfStreamMarker));
            
            return message.Substring(0, message.IndexOf(EndOfStreamMarker));
        }

        private async Task CloseSocketAsync(Socket socket)
        {
            await Task.Run(() =>
            {
                Common.TryCatchAction(
                    () => socket.Shutdown(SocketShutdown.Both),
                    Logger, $"Error occured while shuting down socket to {((IPEndPoint)socket?.RemoteEndPoint)?.Address.ToString() ?? "UNDEFINED"}"
                    );
                socket.Close();
            }).ConfigureAwait(false);
        }
    }
}
