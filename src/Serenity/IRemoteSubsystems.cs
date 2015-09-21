using System.Collections.Generic;

namespace Serenity
{
    public interface IRemoteSubsystems
    {
        RemoteSubSystem RemoteSubSystemFor(string name);
        IEnumerable<RemoteSubSystem> RemoteSubSystems { get; }
    }
}