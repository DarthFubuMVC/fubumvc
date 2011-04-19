using System;
using System.Diagnostics;
using FubuCore;

namespace Bottles.Deployment.Deployers
{
    public class WindowsSchedulerDeployer : IDeployer<ScheduledTask>
    {
        private IProcessRunner _runner;

        public WindowsSchedulerDeployer(IProcessRunner runner)
        {
            _runner = runner;
        }

        public void Deploy(IDirective directive)
        {
            var d = (ScheduledTask) directive;

            var psi = new ProcessStartInfo("schtasks");

            var args = @"/create /tn {0} /sc {1} /mo {2} /ru {3} /tr {4};".ToFormat(d.Name, d.ScheduleType, d.Modifier, d.UserAccount, d.TaskToRun);
            psi.Arguments = args;

            //log this here

            _runner.Run(psi, new TimeSpan(0, 0, 1, 0));
        }
    }
}