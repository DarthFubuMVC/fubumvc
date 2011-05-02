using FubuCore;

namespace Bottles.Deployment
{
    public class DeploymentSettings
    {
        //path points to ~/deployment
        public DeploymentSettings(string path)
        {
            // TODO -- come back to this.
            //var finder = new DeploymentFolderFinder(new FileSystem());
            //path = finder.FindDeploymentFolder(path);

            DeploymentDirectory = path;
            BottlesDirectory = FileSystem.Combine(path, ProfileFiles.BottlesDirectory);
            RecipesDirectory = FileSystem.Combine(path, ProfileFiles.RecipesDirectory);
            EnvironmentFile = FileSystem.Combine(path, ProfileFiles.EnvironmentSettingsFileName);
            TargetDirectory = FileSystem.Combine(path, ProfileFiles.TargetDirectory);
            BottleManifestFile = FileSystem.Combine(path, ProfileFiles.BottlesManifestFile);

            
        }

        public static DeploymentSettings ForRootDirectory()
        {
            var path = FileSystem.Combine(".".ToFullPath(), ProfileFiles.DeploymentFolder);
            return new DeploymentSettings(path);
        }

        public DeploymentSettings() : this(".".ToFullPath())
        {
        }

        public string DeploymentDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string BottlesDirectory { get; set;}
        public string RecipesDirectory { get; set;}
        public string EnvironmentFile { get; set;}
        public string BottleManifestFile { get; set; }

        public string StagingDirectory
        {
            get
            {
                return FileSystem.Combine(TargetDirectory, ProfileFiles.StagingDirectory);
            }
        }

        /// <summary>
        /// user has typed '-f' at the command line
        /// </summary>
        public bool UserForced { get; set; }

        

        public string GetRecipe(string recipe)
        {
            return FileSystem.Combine(RecipesDirectory, recipe);
        }

        public string GetHost(string recipe, string host)
        {
            var p = GetRecipe(recipe);

            //TODO: harden
            p = FileSystem.Combine(p, host + ".host");

            return p;
        }
    }
}