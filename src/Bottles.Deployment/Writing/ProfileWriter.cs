using System.Collections.Generic;
using FubuCore;

namespace Bottles.Deployment.Writing
{
    public class ProfileWriter
    {
        public void WriteTo(ProfileDefinition profile, DeploymentSettings settings)
        {
            var filename = settings.GetProfile(profile.Name);
            new FileSystem().WriteToFlatFile(filename, writer =>
            {
                profile.Recipes.Each(r => writer.WriteLine(Profile.RecipePrefix + r));

                profile.Values.Each(v => v.Write(writer));
            });
        }
    }
}