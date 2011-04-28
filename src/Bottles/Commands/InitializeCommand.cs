using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Commands
{
    public class InitializeInput
    {
        [Description("Physical folder (or valid alias) of the application")]
        public string DeploymentFlag { get; set; }

        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }

    [CommandDescription("Seeds the /deployment folder structure underneath the root directory of a codebase", Name="init")]
    public class InitializeCommand : FubuCommand<InitializeInput>
    {
        public static readonly string DIRECTORY_ALREADY_EXISTS = "Directory {0} already exists. Use the -f flag to overwrite the existing structure";
        public static readonly string DELETING_EXISTING_DIRECTORY = "Deleting existing deployment directory at {0}";

        public override bool Execute(InitializeInput input)
        {
            //REVIEW: does this need to be here? should this be here?
            if (input.DeploymentFlag != null) input.DeploymentFlag = AliasCommand.AliasFolder(input.DeploymentFlag);

            return Initialize(input, new FileSystem(), new SimpleLogger());
        }

        public bool Initialize(InitializeInput input, IFileSystem fileSystem, ISimpleLogger logger)
        {
            var deploymentDirectory = FileSystem.Combine(input.DeploymentRoot(), ProfileFiles.DeploymentFolder);
            logger.Log("Trying to initialize Bottles deployment folders at {0}", deploymentDirectory);

            if (fileSystem.DirectoryExists(deploymentDirectory))
            {
                if (input.ForceFlag)
                {
                    logger.Log(DELETING_EXISTING_DIRECTORY, deploymentDirectory);
                    fileSystem.DeleteDirectory(deploymentDirectory);
                }
                else
                {
                    logger.Log(DIRECTORY_ALREADY_EXISTS, deploymentDirectory);
                    return false;
                }
            }

            createDirectory(fileSystem, logger, deploymentDirectory);

            fileSystem.WriteStringToFile(FileSystem.Combine(deploymentDirectory, ProfileFiles.BottlesManifestFile), "");

            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.BottlesDirectory);
            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.RecipesDirectory);
            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.EnvironmentsFolder);
            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.ProfilesFolder);

            return true;
        }

        private void createDirectory(IFileSystem system, ISimpleLogger logger, params string[] pathParts)
        {
            string directory = FileSystem.Combine(pathParts);

            logger.Log("Creating directory " + directory);
            system.CreateDirectory(directory);
        }
    }


}