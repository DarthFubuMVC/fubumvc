using System;
using System.Diagnostics;
using System.IO;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace FubuMVC.TestingHarness
{
    public class CommandRunner
    {
        private readonly string _solutionDirectory;
        private readonly string _applicationDirectory;

        public CommandRunner()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            // Assuming that the assembly will be running in Fubu centric
            // [solution]/src/[library]/bin/debug
            _applicationDirectory = path.ParentDirectory().ParentDirectory();
            _solutionDirectory = _applicationDirectory.ParentDirectory().ParentDirectory();
        }

        public void RunBottles(string commandLine)
        {
            var fileName = Path.Combine(_solutionDirectory, @"bottles.cmd");
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

        public void RemoveAllLinks()
        {
            new LinkCommand().Execute(new LinkInput{
                AppFolder = _applicationDirectory,
                CleanAllFlag = true
            });
        }

    }
}