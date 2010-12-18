using System.Collections.Generic;

namespace FubuMVC.Core.Packaging.Environment
{
    public interface IEnvironmentGateway
    {
        IEnumerable<LogEntry> Install();
        IEnumerable<LogEntry> CheckEnvironment();
        IEnumerable<LogEntry> InstallAndCheckEnvironment();
    }
}