using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging.Environment
{
    public class EnvironmentProxy : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private IEnumerable<LogEntry> execute(EnvironmentRun run, params Action<IInstaller, IPackageLog>[] actions)
        {
            var runner = new EnvironmentRunner(run);
            return runner.ExecuteEnvironment(actions);
        }

        public IEnumerable<LogEntry> Install(EnvironmentRun run)
        {
            return execute(run, (i, l) => i.Install(l));
        }

        public IEnumerable<LogEntry> CheckEnvironment(EnvironmentRun run)
        {
            return execute(run, (i, l) => i.CheckEnvironment(l));
        }

        public IEnumerable<LogEntry> InstallAndCheckEnvironment(EnvironmentRun run)
        {
            return execute(run, (i, l) => i.Install(l), (i, l) => i.CheckEnvironment(l));
        }
    }
}