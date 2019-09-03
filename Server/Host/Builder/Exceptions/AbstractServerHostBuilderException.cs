using System;
using System.Net;

namespace Server.Host.Builder.Exceptions
{
    public abstract class AbstractServerHostBuilderException : Exception
    {
        public string HostName { get; }
        public IPAddress IPAddress { get; }

        public AbstractServerHostBuilderException(string message) : base(message)
        {
            HostName = Dns.GetHostName();
            IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        }

        protected string HostNameAndIPv6ForLogging
        {
            get => String.Format("Host Name: {0}{1}IPv6: {2}{3}",
                            HostName,
                            Environment.NewLine,
                            IPAddress.ToString(),
                            Environment.NewLine
                            );
        }
    }
}
