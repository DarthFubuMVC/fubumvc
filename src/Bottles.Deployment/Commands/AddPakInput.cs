using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class AddPakInput
    {
        [Description("Directory (or alias) of the bottle")]
        public string PackageDirectory { get; set; }

        [Description("Where the ~/deployment folder is")]
        public string DeploymentFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }


    [CommandDescription("Adds the pak (alias) to the bottles.manifest", Name="add-pak")]
    public class AddPakCommand : FubuCommand<AddPakInput>
    {
        public override bool Execute(AddPakInput input)
        {
            var fileSystem = new FileSystem();

            var settings = DeploymentSettings.ForDirectory(input.DeploymentFlag);

            var list = new List<string>();
            fileSystem.ReadTextFile(settings.BottleManifestFile, list.Add);

            list.Fill(input.PackageDirectory);
            list.Sort();

            Console.WriteLine("Adding directory {0} to the bottles manifest at {1}", input.PackageDirectory, settings.BottleManifestFile);
            Console.WriteLine("The current bottles are:");
            list.Each(x => Console.WriteLine("  " + x));

            using (var writer = new StreamWriter(settings.BottleManifestFile))
            {
                list.Each(x => writer.WriteLine(x));
            }

            return true;
        }
    }
}