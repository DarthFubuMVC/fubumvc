using System.Collections.Generic;
using Bottles.Deployment.Writing;
using FubuCore.CommandLine;
using FubuCore;

namespace Bottles.Deployment.Commands
{
    public class AddRecipeInput
    {
        public string Name { get; set; }
        public string ProfileFlag { get; set; }
        public IEnumerable<string> Hosts { get; set; }

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
            var p = new ProfileWriter(input.Profile());
            var r = p.RecipeFor(input.Name);

            input.Hosts.Each(h => r.HostFor(h));

            p.Flush();

            return true;
        }
    }
}