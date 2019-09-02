using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Host;
using Server.Host.Builder;
using Server.Host.Builder.Interfaces;
using Server.Host.HostAbstractions.Interfaces;

namespace Server
{
    public class Startup : AbstractStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<IServerHostBuilder, ServerHostBuilder>();
            services.AddTransient<IServerHostSetup, ServerHost>();

            // Add Console Logging
            services.AddLogging(cfg => cfg.AddConsole());
        }

        public override void Configure()
        {
        }
    }
}
