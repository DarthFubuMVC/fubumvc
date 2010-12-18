using System.Collections.Generic;
using FubuMVC.Core.Packaging.Environment;

namespace Fubu
{
    public interface IInstallationLogger
    {
        void WriteLogsToConsole(IEnumerable<LogEntry> entries);
        void WriteLogsToFile(InstallInput input, IEnumerable<LogEntry> entries);
        void WriteSuccessToConsole();
        void WriteFailureToConsole();
    }
}