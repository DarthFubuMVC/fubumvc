using System;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageLog
    {
        void Trace(string text);
        void Trace(string format, params object[] parameters);
        void MarkFailure(Exception exception);
    }
}