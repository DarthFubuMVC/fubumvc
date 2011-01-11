using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using StoryTeller.Assertions;

namespace IntegrationTesting
{
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
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _solutionDirectory
            };

            try
            {
               
                var process = Process.Start(startup);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    StoryTellerAssert.Fail("Command failed! -- " + commandLine + "\n" + process.StandardOutput.ReadToEnd());
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