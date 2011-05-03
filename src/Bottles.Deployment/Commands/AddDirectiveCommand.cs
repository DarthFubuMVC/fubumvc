using System.ComponentModel;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Reflection;

namespace Bottles.Deployment.Commands
{

    public class AddDirectiveInput
    {
        [Description("The recipe to add the directive to.")]
        public string Recipe { get; set; }

        [Description("The host in the recipe to add the directive to.")]
        public string Host { get; set; }

        [Description("The directive to add.")]
        public string Directive { get; set; }

        [Description("The directory where ")]
        public string DeploymentFlag { get; set; }

        [Description("Open the directive file when done.")]
        public bool OpenFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? @".".ToFullPath();
        }
    }

    [CommandDescription("Adds a directive to an existing /deployment/recipe/host ", Name="add-directive")]
    public class AddDirectiveCommand : FubuCommand<AddDirectiveInput>
    {
        IFileSystem _fileSystem = new FileSystem();

        public override bool Execute(AddDirectiveInput input)
        {
            var settings = new DeploymentSettings(input.DeploymentRoot());
            
            var c = DeploymentBootstrapper.Bootstrap(settings);
            var directiveTypeRegistry = c.GetInstance<IDirectiveTypeRegistry>();
            return Initialize(directiveTypeRegistry, input, settings);
        }

        public bool Initialize(IDirectiveTypeRegistry registry, AddDirectiveInput input, DeploymentSettings settings)
        {
            var rec = settings.GetRecipeDirectory(input.Recipe);

            var host = new HostDefinition(input.Host);
            var type = registry.DirectiveTypeFor(input.Directive);
            var directive = type.Create<IDirective>();

            host.AddDirective(directive);

            var hw = new HostWriter(new TypeDescriptorCache());
            hw.WriteTo(host, rec);

            if(input.OpenFlag)
                _fileSystem.LaunchEditor(rec, host.FileName);

            return true;
        }


    }
}