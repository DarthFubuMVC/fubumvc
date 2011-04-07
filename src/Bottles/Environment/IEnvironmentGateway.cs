using System.Collections.Generic;

namespace Bottles.Environment
{
    public interface IEnvironmentGateway
    {
        IEnumerable<LogEntry> Install();
        IEnumerable<LogEntry> CheckEnvironment();
        IEnumerable<LogEntry> InstallAndCheckEnvironment();
    }
}