using System;
using System.ComponentModel;
using Bottles.Deployment;
using Bottles.Deployment.Commands;
using Bottles.Deployment.Writing;
using FubuCore.CommandLine;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Deployers
{
    public class CreateDeploymentInput
    {
        public CreateDeploymentInput()
        {
            HostFlag = "web";
            RecipeFlag = "baseline";
        }

        [Description("Declares the main application bottle")]
        public string ApplicationBottleName { get; set; }

        [Description("Recipe name for the FubuWebsite.  Default is 'baseline'")]
        public string RecipeFlag { get; set; }

        [Description("Host name of the FubuWebsite directive.  Default is 'web'")]
        public string HostFlag { get; set; }

        [Description("Override the virtual directory name")]
        public string VirtualDirFlag { get; set; }

        [Description("Overrides the location of the ~/deployment directory")]
        public string DeploymentFlag { get; set; }

        [Description("Open the directive file when done.")]
        public bool OpenFlag { get; set; }
    }

    [CommandDescription("Creates or seeds a new website deployment", Name = "create-deployment")]
    public class CreateDeploymentCommand : FubuCommand<CreateDeploymentInput>
    {
        public override bool Execute(CreateDeploymentInput input)
        {
            var settings = DeploymentSettings.ForDirectory(input.DeploymentFlag);
            var directive = new FubuWebsite();

            if (input.VirtualDirFlag.IsNotEmpty())
            {
                directive.VDir = input.VirtualDirFlag;
            }

            var fileSystem = new FileSystem();

            var hostFile = settings.GetHost(input.RecipeFlag, input.HostFlag);
            Console.WriteLine("Adding a new FubuWebsite directive to " + hostFile);

            fileSystem.WriteToFlatFile(hostFile, file =>
            {
                var writer = new DirectiveWriter(file, new TypeDescriptorCache());
                writer.Write(directive);
            });

            Console.WriteLine("");

            new ReferenceBottleCommand().Execute(new ReferenceBottleInput()
            {
                Bottle = input.ApplicationBottleName,
                Host = input.HostFlag,
                Recipe = input.RecipeFlag
            });

            if (input.OpenFlag)
            {
                fileSystem.LaunchEditor(hostFile);
            }

            return true;
        }
    }
}