using FubuCore;

namespace Bottles.Deployment
{
    public class DeploymentSettings
    {
        public DeploymentSettings(string path)
        {
            BottlesDirectory = FileSystem.Combine(path, ProfileFiles.BottlesDirectory);
            RecipesDirectory = FileSystem.Combine(path, ProfileFiles.RecipesDirectory);
            EnvironmentFile = FileSystem.Combine(path, ProfileFiles.EnvironmentSettingsFileName);

            TargetDirectory = FileSystem.Combine(path, ProfileFiles.TargetDirectory);
        }
        public DeploymentSettings() : this(".".ToFullPath())
        {
        }

        public string TargetDirectory { get; set; }
        public string BottlesDirectory { get; set;}
        public string RecipesDirectory { get; set;}
        public string EnvironmentFile { get; set;}

        /// <summary>
        /// user has typed '-f' at the command line
        /// </summary>
        public bool UserForced { get; set; }
    }
}