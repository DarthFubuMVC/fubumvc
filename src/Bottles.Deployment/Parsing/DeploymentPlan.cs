using System.Collections.Generic;
using Bottles.Configuration;

namespace Bottles.Deployment.Parsing
{
    public class DeploymentPlan
    {
        public IEnumerable<Recipe> Recipes { get; private set; }
        public IEnumerable<HostManifest> Hosts { get; set; }
        public Profile Profile { get; private set; }
        public EnvironmentSettings Environment { get; private set; }

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

        public void SetEnv(EnvironmentSettings environment)
        {
            Environment = environment;
        }

        public void CombineOverrides()
        {
            Profile.Overrides.Each((k, v) =>
            {
                Environment.Overrides[k] = v;
            });
        }
    }
}