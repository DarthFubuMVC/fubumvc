namespace Bottles.Deployment
{
    public class DeploymentSettings
    {
        public DeploymentSettings()
        {
            BottlesDirectory = "bottles";
            RecipesDirectory = ProfileFiles.RecipesFolder;
            ProfileFile = ProfileFiles.EnvironmentSettingsFileName;
        }

        public string BottlesDirectory { get; set;}
        public string RecipesDirectory { get; set;}
        public string ProfileFile { get; set;}
    }
}