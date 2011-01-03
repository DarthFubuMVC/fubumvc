using System;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;

namespace Fubu
{
    [CommandDescription("Runs installer actions and/or environment checks for an application")]
    public class InstallCommand : FubuCommand<InstallInput>
    {
        public override void Execute(InstallInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            Execute(input, new FileSystem());
        }

        public void Execute(InstallInput input, IFileSystem fileSystem)
        {
            if (!fileSystem.FileExists(input.ManifestFileName))
            {
                WriteApplicationManifestDoesNotExist(input.AppFolder);
                return;
            }

            InstallManifest(input, fileSystem);
        }

        public virtual void WriteApplicationManifestDoesNotExist(string appFolder)
        {
            Console.WriteLine("No Application Manifest file at {0}. Run 'fubu manifest [folder] -create' first",
                              appFolder);
        }

        public virtual void InstallManifest(InstallInput input, IFileSystem fileSystem)
        {
            Console.WriteLine("Executing the installers for the FubuMVC application at {0}", input.AppFolder);

            var manifest = fileSystem.LoadFromFile<ApplicationManifest>(input.ManifestFileName);
            var run = CreateEnvironmentRun(input, manifest);

            try
            {
                run.AssertIsValid();
            }
            catch (EnvironmentRunnerException e)
            {
                Console.WriteLine("The application manifest is either incomplete or invalid");
                Console.WriteLine(e.Message);

                throw;
            }

            RunTheEnvironment(input, new EnvironmentGateway(run));
        }


        public virtual void RunTheEnvironment(InstallInput input, IEnvironmentGateway gateway)
        {
            var runner = new InstallationRunner(gateway, new InstallationLogger());
            runner.RunTheInstallation(input);
        }

        public virtual void WriteEnvironmentRunIsInvalid(string message)
        {
            Console.WriteLine("Application Manifest file is incomplete or invalid");
            Console.WriteLine(message);
        }

        public static EnvironmentRun CreateEnvironmentRun(InstallInput input, ApplicationManifest manifest)
        {
            var binFolder = FileSystem.Combine(input.AppFolder, "bin").ToFullPath();
            var configFile = manifest.ConfigurationFile ?? "web.config";
            configFile = Path.GetFileName(configFile);
            configFile = FileSystem.Combine(input.AppFolder, configFile).ToFullPath();

            return new EnvironmentRun{
                ApplicationBase = binFolder,
                AssemblyName = manifest.EnvironmentAssembly,
                EnvironmentClassName = manifest.EnvironmentClassName,
                ConfigurationFile = configFile,
                ApplicationDirectory = input.AppFolder.ToFullPath()
            };
        }
    }
}