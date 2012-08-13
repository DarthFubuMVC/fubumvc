using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;

namespace FubuMVC.TestingHarness
{
    public class CommandRunner
    {
        private readonly string _solutionDirectory;

        public CommandRunner()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            var fileSystem = new FileSystem();


            var isFound = fileSystem.FileExists(path, @"src\fubu\bin\debug", "fubu.exe");
            while (!isFound)
            {
                path = Path.Combine(path, "..");
                isFound = fileSystem.FileExists(path, @"src\fubu\bin\debug", "fubu.exe");
            }

            _solutionDirectory = Path.GetFullPath(path);
        }

        public void RunBottles(string commandLine)
        {
            var fileName = Path.Combine(_solutionDirectory, @"bottles.cmd");
            Debug.WriteLine("Execute: {0} {1}".ToFormat(fileName, commandLine));
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
                var processOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new CommandFailureException("Command failed! -- " + commandLine + "\n" + processOutput);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Trying to run " + fileName, e);
            }
        }

        public void RunFubu(string commandLine)
        {
            var fileName = Path.Combine(_solutionDirectory, @"src\fubu\bin\debug\fubu.exe");
            Debug.WriteLine("Execute: {0} {1}".ToFormat(fileName, commandLine));
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
                var processOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Debug.WriteLine(processOutput);

                if (process.ExitCode != 0)
                {
                    throw new CommandFailureException("Command failed! -- " + commandLine + "\n" + processOutput);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Trying to run " + fileName, e);
            }
        }
    }
}