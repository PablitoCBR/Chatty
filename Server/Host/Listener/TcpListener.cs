using System;
using System.Collections.Concurrent;
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
        private IPEndPoint IPEndPoint { get; }
        private int BufferSize { get; }
        private int ConnectionsQueueLength { get; }
        private string EndOfStreamMarker { get; }

        public ProtocolType ProtocolType { get => ProtocolType.Tcp; }

        private CancellationTokenSource ListeningCancelationToken { get; set; }
        private Socket Listener { get; set; }
        private ConcurrentDictionary<Socket, CancellationTokenSource> ConnectionHandlers { get; }
        public bool IsListening { get; private set; }


        public TcpListener(IPAddress ipAddress, ushort port, int bufferSize, int pendingConnectionQueueLength,
            string endOfStreamMarker, ILogger<TcpListener> logger)
        {
            Logger = logger;
            IPEndPoint = new IPEndPoint(ipAddress, port);
            IsListening = false;

            BufferSize = bufferSize;
            ConnectionsQueueLength = pendingConnectionQueueLength;
            EndOfStreamMarker = endOfStreamMarker;

            ConnectionHandlers = new ConcurrentDictionary<Socket, CancellationTokenSource>();
        }

        public void Listen()
        {
            Listener = new Socket(IPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(IPEndPoint);
            ListeningCancelationToken = new CancellationTokenSource();
            Listen(ListeningCancelationToken.Token);
        }

        public void Stop()
        {
            ListeningCancelationToken.Cancel();
            ListeningCancelationToken.Dispose();
            CloseAllCurrentConnectionHandlers(true);

            if (Listener.Connected)
            {
                Listener.Shutdown(SocketShutdown.Both);
                Listener.Disconnect(false);
            }

            Listener.Close();
            IsListening = false;
        }

        ///----------------------------------------- PRIVATE METOHDS -----------------------------------------------------------///

        private void Listen(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting listening with TCP on port: {0}", IPEndPoint.Port);
            Listener.Listen(ConnectionsQueueLength);
            IsListening = true;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                try
                {
                    Socket clientSocket = Listener.Accept();
                    HandleIncomingDataAsync(clientSocket).ConfigureAwait(false);
                }
                catch (SocketException socketExeption)
                {
                    if (socketExeption.SocketErrorCode == SocketError.Interrupted && cancellationToken.IsCancellationRequested)
                        Logger.LogInformation("TCP stopped listening due to cancellation token.");
                    else Logger.LogCritical(socketExeption, "TCP Listener stopped listening due to unhandled exception!");
                }
            }
        }

        private async Task HandleIncomingDataAsync(Socket clientSocket)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            if (!ConnectionHandlers.TryAdd(clientSocket, cts))
            {
                Logger.LogError("Failed to add connection handler to Dict. IPv6: {0}",
                    ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
                cts.Dispose();
                return;
            }

            try
            {
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                string messageReceived = await ReceiveDataAsync(clientSocket, cts.Token).ConfigureAwait(false);

                if (String.IsNullOrEmpty(messageReceived))
                    await clientSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes("TIMEOUT")), SocketFlags.None);
                else await HandleMessageAsync(messageReceived, clientSocket).ConfigureAwait(false);
            }
            catch (AggregateException aggregateException)
            {
                foreach (Exception ex in aggregateException.InnerExceptions)
                    Logger.LogError(ex, "Exception occured while receiving data from {0}",
                        ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
            }
            finally
            {
                await CloseSocketAsync(clientSocket).ConfigureAwait(false);
                if (ConnectionHandlers.TryRemove(clientSocket, out cts))
                    cts.Dispose();
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
                        Logger.LogWarning("TCP connection canceled after 5s timeout {0}Remote IPv6: {1}",
                            Environment.NewLine, ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
                        return await Task.FromResult(String.Empty);
                    }
                    await clientSocket.ReceiveAsync(buffer, SocketFlags.None).ConfigureAwait(false);
                    message += Encoding.ASCII.GetString(buffer);
                }
                catch (AggregateException ex)
                {
                    Logger.LogError("Data receiving from {0} has to stopped due to exception",
                        ((IPEndPoint)clientSocket?.RemoteEndPoint)?.Address.ToString());
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

        private void CloseAllCurrentConnectionHandlers(bool dispose)
        {
            foreach (var handler in ConnectionHandlers)
            {
                handler.Value.Cancel();

                if (handler.Key.Connected)
                    handler.Key.Shutdown(SocketShutdown.Both);

                handler.Key.Close();

                if (dispose)
                {
                    handler.Value.Dispose();
                    handler.Key.Dispose();
                }
            }

            ConnectionHandlers.Clear();
        }

        ///--------------------------------------------------- IDisposable ----------------------------------------------------------------///

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            CloseAllCurrentConnectionHandlers(dispose);

            if (Listener.Connected)
                Listener.Shutdown(SocketShutdown.Both);

            if (dispose)
                Listener.Close();
        }









        /// TEMP METHOD !!! Will be replaced by class handling recived data (extracting recepient and passing message)
        private Task HandleMessageAsync(string message, Socket clientSocket)
        {
            Logger.LogInformation("Recived message: {0}", message);
            clientSocket.Send(Encoding.ASCII.GetBytes(message));
            return Task.CompletedTask;
        }
    }
}
