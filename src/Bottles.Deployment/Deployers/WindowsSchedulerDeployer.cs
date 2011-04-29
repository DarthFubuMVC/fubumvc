using System;
using System.Diagnostics;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Deployment.Deployers
{
    public class WindowsSchedulerDeployer : IDeployer<ScheduledTask>
    {
        private readonly IProcessRunner _runner;

        public WindowsSchedulerDeployer(IProcessRunner runner)
        {
            _runner = runner;
        }

        public void Execute(ScheduledTask directive, HostManifest host, IPackageLog log)
        {
            var psi = new ProcessStartInfo("schtasks");

            var args = @"/create /tn {0} /sc {1} /mo {2} /ru {3} /tr {4};".ToFormat(directive.Name, directive.ScheduleType, directive.Modifier, directive.UserAccount, directive.TaskToRun);
            psi.Arguments = args;

            log.Trace(args);

            _runner.Run(psi, new TimeSpan(0, 0, 1, 0));
        }

    }
}