using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;
using System.Linq;

namespace Fubu
{
    public enum InstallMode
    {
        install,
        all,
        check
    }

    public class InstallInput
    {
        public InstallInput()
        {
            ModeFlag = InstallMode.install;
            LogFileFlag = "installation.htm";
        }

        public string AppFolder { get; set; }
        public InstallMode ModeFlag { get; set; }
        public string LogFileFlag { get; set; }
        public bool OpenFlag { get; set; }

        public string ManifestFileName
        {
            get { return Path.GetFullPath(FileSystem.Combine(AppFolder, ApplicationManifest.FILE)); }
        }

        public string Title()
        {
            string format = "";

            switch (ModeFlag)
            {
                case InstallMode.install:
                    format = "Installing the application at {0}";
                    break;

                case InstallMode.check:
                    format = "Running environment checks for {0}";
                    break;

                case InstallMode.all:
                    format = "Installing and running environment checks for {0}";
                    break;
            }

            return format.ToFormat(ManifestFileName);
        }
    }

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
                Console.WriteLine("No Application Manifest file at {0}. Use 'fubu manifest [folder] -create' first");
                return;
            }

            // TODO -- harden
            var manifest = fileSystem.LoadFromFile<ApplicationManifest>(input.ManifestFileName);
            installManifest(input, manifest);
        }

        private void installManifest(InstallInput input, ApplicationManifest manifest)
        {
            var binFolder = FileSystem.Combine(input.AppFolder, "bin").ToFullPath();
            var configFile = manifest.ConfigurationFile ?? "web.config";
            configFile = Path.GetFileName(configFile);
            configFile = FileSystem.Combine(input.AppFolder, configFile).ToFullPath();

            var run = new EnvironmentRun(){
                ApplicationBase = binFolder,
                AssemblyName = manifest.EnvironmentAssembly,
                EnvironmentClassName = manifest.EnvironmentClassName,
                ConfigurationFile = configFile
            };

            // TODO -- Harden around this
            run.AssertIsValid();

            // TODO -- Harden
            var domain = new EnvironmentGateway(run);
            var entries = execute(domain, input);

            entries.Each(log =>
            {
                Console.WriteLine("{0}, Success = {1}", log.Description, log.Success);
                Console.WriteLine(log.TraceText);

                Console.WriteLine();
                Console.WriteLine();
            });

            var success = entries.Any(x => !x.Success);
            Console.WriteLine(success ? "Success!" : "Failure!");



            var document = EntryLogWriter.Write(entries, input.Title() + " at " + DateTime.UtcNow.ToLongDateString());
            document.WriteToFile(input.LogFileFlag);

            Console.WriteLine("Output writting to {0}", Path.GetFullPath(input.LogFileFlag));

            if (input.OpenFlag)
            {
                document.OpenInBrowser();
            }
        }

        private IEnumerable<LogEntry> execute(EnvironmentGateway gateway, InstallInput input)
        {
            Console.WriteLine(input.Title());

            switch (input.ModeFlag)
            {
                case InstallMode.install:
                    return gateway.Install();

                case InstallMode.check:
                    return gateway.CheckEnvironment();

                case InstallMode.all:
                    return gateway.InstallAndCheckEnvironment();


            }

            return new LogEntry[0];
        }
    }
}