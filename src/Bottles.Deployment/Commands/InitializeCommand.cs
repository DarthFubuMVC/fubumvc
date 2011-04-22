using System;
using System.ComponentModel;
using System.IO;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{
    public class InitializeInput
    {
        [Description("Physical folder (or valid alias) of the application")]
        public string DirectoryFlag { get; set; }

        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        public string RootDirectory()
        {
            return DirectoryFlag ?? ".".ToFullPath();
        }
    }

    [CommandDescription("Seeds the /deployment folder structure underneath the root directory of a codebase")]
    public class InitializeCommand : FubuCommand<InitializeInput>
    {
        public static readonly string DIRECTORY_ALREADY_EXISTS = "Directory {0} already exists. Use the -f flag to overwrite the existing structure";
        public static readonly string DELETING_EXISTING_DIRECTORY = "Deleting existing deployment directory at {0}";

        public override bool Execute(InitializeInput input)
        {
            if (input.DirectoryFlag != null) input.DirectoryFlag = AliasCommand.AliasFolder(input.DirectoryFlag);

            return Initialize(input, new FileSystem(), new SimpleLogger());
        }

        public bool Initialize(InitializeInput input, IFileSystem fileSystem, ISimpleLogger logger)
        {
            var deploymentDirectory = FileSystem.Combine(input.RootDirectory(), ProfileFiles.DeploymentFolder);
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
            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.BottlesFolder);
            createDirectory(fileSystem, logger, deploymentDirectory, ProfileFiles.RecipesFolder);
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