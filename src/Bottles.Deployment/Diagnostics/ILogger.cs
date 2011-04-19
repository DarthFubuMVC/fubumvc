using System;

namespace Bottles.Deployment.Diagnostics
{
    public interface ILogger
    {
        void Log(string providence, Action action);
        void LogHost(HostManifest hostManifest, Action<HostManifest> action);
    }
}