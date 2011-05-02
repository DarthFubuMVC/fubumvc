using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var settings = DeploymentSettings.ForDirectory(input.DeploymentFlag);

            new FileSystem().AlterFlatFile(settings.BottleManifestFile, list =>
            {
                list.Fill(input.PackageDirectory);
                list.Sort();

                Console.WriteLine("Adding directory {0} to the bottles manifest at {1}", input.PackageDirectory, settings.BottleManifestFile);
                Console.WriteLine("The current bottles are:");
                list.Each(x => Console.WriteLine("  " + x));
            });

            return true;
        }
    }
}