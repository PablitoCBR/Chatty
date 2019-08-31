using Server.Host.HostAbstractions.Interfaces;

namespace Server.Host.HostAbstarctions
{
    public abstract class AbstractServerHost : IServerHostSetup
    {
        public virtual void Run()
        {
        }
    }
}
