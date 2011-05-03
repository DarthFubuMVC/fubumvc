using System;
using System.ComponentModel;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Writing;
using FubuCore.CommandLine;
using FubuCore;

namespace Fubu
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

            // write the directive
            // write the bottle reference
            //var writer = new DirectiveWriter()
        
            throw new NotImplementedException();
        }
    }
}