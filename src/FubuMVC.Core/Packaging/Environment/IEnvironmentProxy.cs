using System.Collections.Generic;

namespace FubuMVC.Core.Packaging.Environment
{
    public interface IEnvironmentProxy
    {
        IEnumerable<LogEntry> Install();
        IEnumerable<LogEntry> CheckEnvironment();
        IEnumerable<LogEntry> InstallAndCheckEnvironment();
    }
}