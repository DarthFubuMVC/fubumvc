using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FubuMVC.Core.Packaging.Environment;
using FubuTestApplication;
using System.Linq;
using FubuCore;
using StoryTeller.Assertions;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Packages
{
    public class EnvironmentRunnerFixture : Fixture
    {
        private IEnumerable<LogEntry> _installLogs;
        private IEnumerable<LogEntry> _environmentLogs;
        private IEnumerable<LogEntry> _allLogs;

        public EnvironmentRunnerFixture()
        {
            SelectionValuesFor("environments").AddRange(findEnvironments());

            this["InstallLogs"] = VerifySetOf(() => _installLogs).Titled("The log entries for installation should be")
                .MatchOn(x => x.Description, x => x.Success, x => x.TraceText);

            this["EnvironmentLogs"] = VerifySetOf(() => _environmentLogs).Titled("The log entries for checking the environment should be")
                .MatchOn(x => x.Description, x => x.Success, x => x.TraceText);

            this["InstallAndEnvironmentLogs"] = VerifySetOf(() => _allLogs).Titled("The log entries for both installation and checking the environment should be")
                .MatchOn(x => x.Description, x => x.Success, x => x.TraceText);
        }

        [FormatAs("For environment {name}")]
        public void ForEnvironment([SelectionValues("environments")]string name)
        {
            var className = "{0}.{1}, {0}".ToFormat(typeof (EnvironmentThatBlowsUpInStartUp).Assembly.GetName().Name,
                                                    name);

            var applicationBase = Path.GetFullPath(@"src/FubuTestApplication/bin");

            Debug.WriteLine("Setting the Application Base to " + applicationBase);

            var run = new EnvironmentRun(){
                ApplicationBase = applicationBase,
                EnvironmentClassName = className
            };

            var domain = new EnvironmentGateway(run);
            _installLogs = domain.Install();
            _environmentLogs = domain.CheckEnvironment();
            _allLogs = domain.InstallAndCheckEnvironment();
        }

        [FormatAs("Installation log entry with description {description} is marked as a failure and should contain the text {trace}")]
        public bool FailedInstallLogForContains(string description, string trace)
        {
            var log = _installLogs.FirstOrDefault(x => x.Description == description);
            StoryTellerAssert.Fail(log == null, () => "Could not find the log entry with that description.  Possibles are:\n" + _installLogs.Select(x => x.Description).Join(";\n"));

            return log.TraceText.Contains(trace);
        }


        [FormatAs("Check Environment log entry with description {description} is marked as a failure and should contain the text {trace}")]
        public bool FailedCheckEnvironmentLogForContains(string description, string trace)
        {
            var log = _environmentLogs.FirstOrDefault(x => x.Description == description);
            StoryTellerAssert.Fail(log == null, () => "Could not find the log entry with that description.  Possibles are:\n" + _environmentLogs.Select(x => x.Description).Join(";\n"));

            return log.TraceText.Contains(trace);
        }

        private static IEnumerable<string> findEnvironments()
        {
            return
                typeof (EnvironmentThatBlowsUpInStartUp).Assembly.GetExportedTypes().Where(
                    x => x.CanBeCastTo<IEnvironment>()).Select(x => x.Name);
        }
    }

    

    
}