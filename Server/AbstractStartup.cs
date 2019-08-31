using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Server
{
    public abstract class AbstractStartup
    {
        public IConfiguration Configuration { get; }

        public AbstractStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure()
        {

        }
    }
}
