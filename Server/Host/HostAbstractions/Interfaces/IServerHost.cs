using System;

namespace Server.Host.HostAbstractions.Interfaces
{
    public interface IServerHost : IDisposable
    {
        void Run();
        void StopListeners();
        void StartListeners();
    }
}
