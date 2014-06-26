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
        private readonly IPackageService _packaging = new PackageService(new FileSystem());

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


        public void CleanAndRemoveAllPackages()
        {
            _packaging.CleanAllPackages(_applicationDirectory);
            _packaging.RemoveAllPackages(_applicationDirectory);
        }

        public void RemoveAllLinks()
        {
            new LinkCommand().Execute(new LinkInput{
                AppFolder = _applicationDirectory,
                CleanAllFlag = true
            });
        }

        public void InstallZipPackage(string zipFile)
        {
            _packaging.InstallPackage(_applicationDirectory, _solutionDirectory.AppendPath(zipFile), false);
        }

        public void UnInstallZipPackage(string zipFile)
        {
            _packaging.InstallPackage(_applicationDirectory, _solutionDirectory.AppendPath(zipFile), true);
        }
    }
}