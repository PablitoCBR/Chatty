using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Host;
using Server.Host.Builder;
using Server.Host.Builder.Interfaces;
using Server.Host.HostAbstarctions;
using Server.Host.HostAbstractions.Interfaces;

namespace Server
{
    public class Startup : AbstractStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<AbstractServerHost, ServerHost>();

            // Add Console Logging
            services.AddLogging(cfg => cfg.AddConsole());

            // Add ServerHostBuilder
            services.AddScoped<IServerHostBuilder, ServerHostBuilder>();
        }

        public override void Configure()
        {
        }
    }
}
