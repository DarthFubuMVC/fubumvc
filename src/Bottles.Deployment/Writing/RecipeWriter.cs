using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Deployment.Writing
{
    public class RecipeWriter
    {
        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly TypeDescriptorCache _types;

        public RecipeWriter(TypeDescriptorCache types)
        {
            _types = types;
        }

        //REVIEW: consider making this take a deployment settings
        public void WriteTo(RecipeDefinition recipe, string profileDirectory)
        {
            var df = new DeploymentFolderFinder(_fileSystem);
            var path = df.FindDeploymentFolder(profileDirectory);
            var recipepath = FileSystem.Combine(path, ProfileFiles.RecipesDirectory, recipe.Name);


            _fileSystem.CreateDirectory(recipepath);

            var controlFilePath = FileSystem.Combine(recipepath, ProfileFiles.RecipesControlFile);
            _fileSystem.WriteStringToFile(controlFilePath, "");

            recipe.Dependencies.Each(d =>
            {
                var line = "Dependency:{0}\n".ToFormat(d);
                _fileSystem.AppendStringToFile(controlFilePath, line);
            });

            recipe.Hosts().Each(host => new HostWriter(_types).WriteTo(host, recipepath));
        }
    }
}