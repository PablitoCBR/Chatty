using Server.Domain.Listener.Interfaces;


namespace Server.Host.HostAbstractions.Interfaces
{
    public interface IServerHostSetup : IServerHost
    {
        void SetListner(IListener listener);
    }
}
