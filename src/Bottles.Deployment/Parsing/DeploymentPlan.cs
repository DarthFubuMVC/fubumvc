using System.Collections.Generic;

namespace Bottles.Deployment.Parsing
{
    public class DeploymentPlan
    {
        public IEnumerable<Recipe> Recipes { get; private set; }
        public IEnumerable<HostManifest> Hosts { get; set; }
        public Profile Profile { get; private set; }

        public void SetProfile(Profile profile)
        {
            Profile = profile;
        }

        public void AddRecipes(IEnumerable<Recipe> recipes)
        {
            Recipes = recipes;
        }

        public void AddHosts(IEnumerable<HostManifest> hosts)
        {
            Hosts = hosts;
        }
    }
}