using CommandLine;
using System;
using System.Net;

namespace Server.Host.Builder
{
    public class BuilderSettings
    {
        [Option('p', "port", Required = false, HelpText = "Specified custom port that application will try to listen on", ResourceType = typeof(ushort))]
        public ushort Port { get; set; }

        [Option('e', "EndOfStreamMarker", Required = false, HelpText = "Set custom marker for end of message stream")]
        public string EndOfStreamMarker { get; set; }

        [Option('t', "TcpPendingConnectionsQueueLength", Required = false, HelpText = "Set max length of pending TCP connections queue", ResourceType = typeof(int))]
        public int TcpPendingConnectionsQueueLength { get; set; }

        [Option("TcpBufferSize", Required = false, HelpText = "TCP port buffer size (bytes). Min value is 8!", ResourceType = typeof(uint))]
        public uint TcpBufferSize { get; set; }

        [Option("UdpBufferSize", Required = false, HelpText = "UDP port buffer size (bytes). Min value is 8!", ResourceType = typeof(uint))]
        public uint UdpBufferSize { get; set; }

        public string HostName { get; set; }
        public IPAddress IpAddress { get; set; }

        public override string ToString()
        {
            string str = String.Empty;
            str += String.Format("Port: {0}{1}", Port, Environment.NewLine);
            str += String.Format("EndOfStreamMarker: {0}{1}", EndOfStreamMarker, Environment.NewLine);
            str += String.Format("TcpPendingConnectionsQueueLength: {0}{1}", TcpPendingConnectionsQueueLength, Environment.NewLine);
            str += String.Format("TcpBufferSize: {0}{1}", TcpBufferSize, Environment.NewLine);
            str += String.Format("UdpBufferSize: {0}{1}", UdpBufferSize, Environment.NewLine);
            str += String.Format("HostName: {0}{1}", HostName, Environment.NewLine);
            str += String.Format("IP Address: {0}{1}", IpAddress.MapToIPv4().ToString(), Environment.NewLine);
            return str;
        }

        public class Options
        {
            public const string Port = "Port";
            public const string TcpPendingConnectionsQueueLength = "TcpPendingConnectionsQueueLength";
            public const string TcpBufferSize = "TcpBufferSize";
            public const string UdpBufferSize = "UdpBufferSize";
            public const string EndOfStreamMarker = "EndOfStreamMarker";
            public const string SectionName = "ServerHostBuilderConfiguration";
        }
    }
}
