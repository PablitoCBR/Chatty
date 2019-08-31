using Server.Host.HostAbstractions.Interfaces;


namespace Server.Host.Builder.Interfaces
{
    public interface IServerHostBuilder
    {
        BuilderSettings Settings { get; set; }
        IServerHost Build();
    }
}
