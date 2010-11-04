using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FubuCore;
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
            _runner = new CommandRunner();
            _runner.RunFubu("createvdir src/FubuTestApplication fubu-testing");
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

        public CommandRunner()
        {
            var path = Environment.CurrentDirectory;

            var fileSystem = new FileSystem();

            bool isFound = fileSystem.FileExists(path, @"src\fubu\bin\debug", "fubu.exe");
            while (!isFound)
            {
                path = Path.Combine(path, "..");
                isFound = fileSystem.FileExists(path, @"src\fubu\bin\debug", "fubu.exe");
            }

            _solutionDirectory = Path.GetFullPath(path);
        }

        public void RunFubu(string commandLine)
        {
            Console.WriteLine("Running 'fubu {0}'", commandLine);

            var fileName = Path.Combine(_solutionDirectory, @"src\fubu\bin\debug\fubu.exe");
            var startup = new ProcessStartInfo(fileName, commandLine){
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                UseShellExecute = false,
                WorkingDirectory = _solutionDirectory
            };

            try
            {
                var process = Process.Start(startup);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    StoryTellerAssert.Fail("Command failed! -- " + commandLine);
                }
            }
            catch (StorytellerAssertionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Trying to run " + fileName, e);
            }
        }
    }
}