using Server.Domain.Listener.Interfaces;

namespace Server.Host.Listener.Interfaces
{
    public interface IListenerFabric
    {
        IListener CreateTCP(ushort port, int pendingConnectionQueue, uint bufferSize, string endOfStreamMarker);
        IListener CreateUDP(ushort port, uint bufferSize);
    }
}
