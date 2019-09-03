using System;
using System.Net.Sockets;

namespace Server.Host.Builder.Exceptions
{
    public class ServerHostBuilderPortException : AbstractServerHostBuilderException
    {
        public uint Port { get; }
        public ProtocolType ProtocolType { get; }

        public ServerHostBuilderPortException(string message) : base(message)
        {
            ProtocolType = ProtocolType.Unspecified;
        }

        public ServerHostBuilderPortException(string message, uint port, ProtocolType protocolType) : this(message)
        {
            Port = port;
            ProtocolType = protocolType;
        }

        public override string ToString()
        {
            string str = String.Format("Server Host Builder Exception. {0}", Environment.NewLine);
            str += String.Format("Port: {0}{1}", (Port != 0) ? Port.ToString() : "Unspecified", Environment.NewLine);

            string protocolType = String.Empty;
            switch (ProtocolType)
            {
                case ProtocolType.Tcp:
                    protocolType = "TCP";
                    break;
                case ProtocolType.Udp:
                    protocolType = "UDP";
                    break;
                case ProtocolType.Unspecified:
                    protocolType = "Unspecified";
                    break;
                default:
                    protocolType = "Unsupported";
                    break;
            }

            str += String.Format("Protocol Type: {0}{1}", protocolType, Environment.NewLine);
            str += base.HostNameAndIPv6ForLogging;
            return str;
        }
    }
}
