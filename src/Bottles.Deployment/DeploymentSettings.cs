using FubuCore;

namespace Bottles.Deployment
{
    public class DeploymentSettings
    {
        public DeploymentSettings(string path)
        {
            BottlesDirectory = FileSystem.Combine(path, ProfileFiles.BottlesDirectory);
            RecipesDirectory = FileSystem.Combine(path, ProfileFiles.RecipesDirectory);
            EnvironmentFile = FileSystem.Combine(ProfileFiles.EnvironmentSettingsFileName);
        }
        public DeploymentSettings()
        {
            BottlesDirectory = ProfileFiles.BottlesDirectory;
            RecipesDirectory = ProfileFiles.RecipesDirectory;
            EnvironmentFile = ProfileFiles.EnvironmentSettingsFileName;
        }

        public string BottlesDirectory { get; set;}
        public string RecipesDirectory { get; set;}
        public string EnvironmentFile { get; set;}

        // user has typed '-f' at the command line
        public bool UserForced { get; set; }
    }
}