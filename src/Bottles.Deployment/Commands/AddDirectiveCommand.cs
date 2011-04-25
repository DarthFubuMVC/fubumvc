using System.ComponentModel;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{

    public class AddDirectiveInput
    {
        [Description("The recipie to add the directive to.")]
        public string Recipe { get; set; }

        [Description("The host in the recipie to add the directive to.")]
        public string Host { get; set; }

        [Description("The directive to add.")]
        public string Directive { get; set; }

        [Description("The directory where ")]
        public string DeploymentFlag { get; set; }

        public string DeploymentLocation()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }

    [CommandDescription("Adds a directive to an existing /deployment/recipie/host ")]
    public class AddDirectiveCommand : FubuCommand<AddDirectiveInput>
    {
        public override bool Execute(AddDirectiveInput input)
        {
            var settings = new DeploymentSettings(input.DeploymentLocation());

            var c = DeploymentBootstrapper.Bootstrap(settings);
            var directiveTypeRegistry = c.GetInstance<IDirectiveTypeRegistry>();
            return Initialize(directiveTypeRegistry, input);
        }

        public bool Initialize(IDirectiveTypeRegistry registry, AddDirectiveInput input)
        {
            var pw = new ProfileWriter(input.DeploymentLocation());

            var recipe = pw.RecipeFor(input.Recipe);
            var host = recipe.HostFor(input.Host);


            var type = registry.DirectiveTypeFor(input.Directive);
            var directive = type.Create<IDirective>();

            host.AddDirective(directive);

            pw.Flush();

            return true;
        }


    }
}