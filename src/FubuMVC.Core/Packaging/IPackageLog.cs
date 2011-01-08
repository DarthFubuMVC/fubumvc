using System;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageLog
    {
        void Trace(string text);
        void Trace(string format, params object[] parameters);
        void MarkFailure(Exception exception);
        void MarkFailure(string text);
    }

    public static class IPackageLogExtensions
    {
        public static void TrapErrors(this IPackageLog log, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                log.MarkFailure(e);
            }
        }
    }
}