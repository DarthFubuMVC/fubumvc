using System;
using System.Diagnostics;
using System.IO;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Assertions;
using StoryTeller.Engine;

namespace StoryTellerTesting
{
    public class FubuSystem : ISystem
    {
        public static readonly string TEST_APPLICATION_ROOT = "http://localhost/fubu-testing";
        private CommandRunner _runner;

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
            var remoteGraph = new RemoteBehaviorGraph(TEST_APPLICATION_ROOT);
            context.Store(remoteGraph);

            context.Store(_runner);
        }

        public void SetupEnvironment()
        {
            // TODO -- make this configurable?
            _runner = new CommandRunner(@"..\..\..\..\");
            _runner.RunFubu("fubu createvdir src/FubuTestApplication fubu-testing");
        }

        public void TeardownEnvironment()
        {
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            registry.AddFixturesFromThisAssembly();
        }
    }

    public class CommandRunner
    {
        private readonly string _solutionDirectory;

        public CommandRunner(string solutionDirectory)
        {
            _solutionDirectory = Path.GetFullPath(solutionDirectory);
        }

        public void RunFubu(string commandLine)
        {
            var fileName = Path.Combine(_solutionDirectory, @"src\fubu\bin\debug\fubu.exe");
            var startup = new ProcessStartInfo(fileName, commandLine){
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _solutionDirectory
            };

            var process = Process.Start(startup);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                StoryTellerAssert.Fail("Command failed! -- " + commandLine);
            }
        }
    }
}