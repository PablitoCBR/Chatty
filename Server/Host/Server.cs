using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Host.Builder.Interfaces;
using System;
using System.IO;
using CommandLine;
using System.Net;
using Microsoft.Extensions.Logging;
using Server.Host.Builder;

namespace Server.Host
{
    public static class Server 
    {
        public static IServerHostBuilder CreateDefaultBuilder<TStartup>(string[] args) where TStartup : AbstractStartup
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(provider => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build()
                );

            TStartup startup = (TStartup)Activator.CreateInstance(typeof(TStartup), services.BuildServiceProvider().GetService(typeof(IConfiguration)));
            startup.ConfigureServices(services);
            startup.Configure();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IServerHostBuilder builder = services.BuildServiceProvider().GetService<IServerHostBuilder>();
            builder.Settings = GetBuilderSettings(args, serviceProvider.GetService<IConfiguration>(), serviceProvider.GetService<ILogger<BuilderSettings>>());

            serviceProvider.GetService<ILogger<BuilderSettings>>().LogInformation("BUILDER SETTINGS {0}{1}", Environment.NewLine, builder.Settings.ToString());
            return builder;
        }

        private static BuilderSettings GetBuilderSettings(string[] args, IConfiguration configuration, ILogger logger)
        {
            BuilderSettings settings = GetDefaultBuilderSettings(configuration);
            if (args.Length == 0)
                return settings;

            Parser.Default.ParseArguments<BuilderSettings>(args)
                .WithParsed(parsed =>
                {
                    logger.LogInformation("Provided parameters parsed successfully and will be used as options for ServerHostBuilder");
                    if (parsed.Port > 0) settings.Port = parsed.Port;
                    if (parsed.TcpPendingConnectionsQueueLength > 0) settings.TcpPendingConnectionsQueueLength = parsed.TcpPendingConnectionsQueueLength;
                    if (parsed.TcpBufferSize > 0)
                    {
                        if (parsed.TcpBufferSize >= 8) settings.TcpBufferSize = parsed.TcpBufferSize;
                        else logger.LogWarning("Tcp buffer size cannot be less than 8 bytes!");
                    }
                    if(parsed.UdpBufferSize > 0)
                    {
                        if (parsed.UdpBufferSize >= 8) settings.UdpBufferSize = parsed.UdpBufferSize;
                        else logger.LogWarning("Udp buffer size cannot be less than 8 bytes!");
                    }
                    if (!String.IsNullOrEmpty(parsed.EndOfStreamMarker)) settings.EndOfStreamMarker = parsed.EndOfStreamMarker;
                })
                .WithNotParsed(errors =>
                {
                    logger.LogError("Failed to parse provided parameters. Default options will be used instead!", errors);
                });

            return settings;
        }

        private static BuilderSettings GetDefaultBuilderSettings(IConfiguration configuration)
        {
            IConfigurationSection HostBuilderConfigurationSection = configuration.GetSection(BuilderSettings.Options.SectionName);
            return new BuilderSettings()
            {
                Port = HostBuilderConfigurationSection.GetValue<ushort>(BuilderSettings.Options.Port),
                EndOfStreamMarker = HostBuilderConfigurationSection.GetValue<string>(BuilderSettings.Options.EndOfStreamMarker),
                TcpPendingConnectionsQueueLength = HostBuilderConfigurationSection.GetValue<int>(BuilderSettings.Options.TcpPendingConnectionsQueueLength),
                TcpBufferSize = HostBuilderConfigurationSection.GetValue<int>(BuilderSettings.Options.TcpBufferSize),
                UdpBufferSize = HostBuilderConfigurationSection.GetValue<int>(BuilderSettings.Options.UdpBufferSize),
                HostName = Dns.GetHostName(),
                IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]
            };
        }
    }
}
