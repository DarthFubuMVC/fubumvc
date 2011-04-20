using System;

namespace Bottles.Deployment.Diagnostics
{
    public interface IPackageDeploymentLog
    {
        void Trace(string text);
        void Trace(string format, params object[] parameters);
        void MarkFailure(Exception exception);
        void MarkFailure(string text);
    }
}