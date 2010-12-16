using System.Collections.Generic;

namespace FubuMVC.Core.Packaging.Environment
{
    public static class EnvironmentListExtensions
    {
        public static void Add(this IList<LogEntry> list, object target, PackageRegistryLog log)
        {
            list.Add(LogEntry.FromPackageLog(target, log));
        }
    }
}