using Server.Host;
using Server.Host.Builder.Interfaces;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateServerHostBuilder(args).Build().Run();
        }

        static IServerHostBuilder CreateServerHostBuilder(string[] args)
            => Host.Server.CreateDefaultBuilder<Startup>(args);
    }
}
