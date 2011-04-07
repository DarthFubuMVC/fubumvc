using System.Collections.Generic;
using Bottles.Environment;

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