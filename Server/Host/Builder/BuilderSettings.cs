using CommandLine;
using System;
using System.Net;

namespace Server.Host.Builder
{
    public class BuilderSettings
    {
        [Option('p', "port", Required = false, HelpText = "Specified custom port that application will try to listen on")]
        public ushort Port { get; set; }

        [Option('e', "EndOfStreamMarker", Required = false, HelpText = "Set custom marker for end of message stream")]
        public string EndOfStreamMarker { get; set; }

        [Option('t', "TcpPendingConnectionsQueueLength", Required = false, HelpText = "Set max length of pending TCP connections queue")]
        public uint TcpPendingConnectionsQueueLength { get; set; }

        [Option('u', "UdpPendingConnectionsQueueLength", Required = false, HelpText = "Set max length of pending UDP connections queue")]
        public uint UdpPendingConnectionsQueueLength { get; set; }
        public string HostName { get; set; }
        public IPAddress IpAddress { get; set; }

        public override string ToString()
        {
            string str = String.Empty;
            str += String.Format("Port: {0}{1}", Port, Environment.NewLine);
            str += String.Format("EndOfStreamMarker: {0}{1}", EndOfStreamMarker, Environment.NewLine);
            str += String.Format("TcpPendingConnectionsQueueLength: {0}{1}", TcpPendingConnectionsQueueLength, Environment.NewLine);
            str += String.Format("UdpPendingConnectionsQueueLength: {0}{1}", UdpPendingConnectionsQueueLength, Environment.NewLine);
            str += String.Format("HostName: {0}{1}", HostName, Environment.NewLine);
            str += String.Format("IP Address: {0}{1}", IpAddress.MapToIPv4().ToString(), Environment.NewLine);
            return str;
        }

        public class Options
        {
            public const string Port = "Port";
            public const string TcpPendingConnectionsQueueLength = "TcpPendingConnectionsQueueLength";
            public const string UdpPendingConnectionsQueueLength = "UdpPendingConnectionsQueueLength";
            public const string EndOfStreamMarker = "EndOfStreamMarker";
            public const string SectionName = "ServerHostBuilderConfiguration";
        }
    }
}
