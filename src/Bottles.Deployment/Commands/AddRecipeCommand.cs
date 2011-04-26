using Bottles.Deployment.Writing;
using FubuCore.CommandLine;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Deployment.Commands
{
    public class AddRecipeInput
    {
        public string Name { get; set; }
        public string ProfileFlag { get; set; }

        public string Profile()
        {
            return ProfileFlag ?? ".".ToFullPath();
        }
    }

    [CommandDescription("Adds a recipe to a profile", Name="add-recipe")]
    public class AddRecipeCommand : FubuCommand<AddRecipeInput>
    {
        public override bool Execute(AddRecipeInput input)
        {
            var path = new DeploymentFolderFinder(new FileSystem()).FindDeploymentFolder(input.Profile());

            var recipe = new RecipeDefinition(input.Name);
            
            var rw = new RecipeWriter(new TypeDescriptorCache());
            rw.WriteTo(recipe, path);


            return true;
        }
    }
}