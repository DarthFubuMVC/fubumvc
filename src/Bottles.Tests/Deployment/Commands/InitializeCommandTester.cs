using System;
using Bottles.Deployment;
using Bottles.Deployment.Commands;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Commands
{


    [TestFixture]
    public class when_the_directory_exists_but_the_force_flag_is_true : InitializeCommandContext
    {
        protected override void theContextIs()
        {
            theDeploymentDirectoryExists();
            theInput.ForceFlag = true;
        }

        [Test]
        public void should_have_deleted_the_existing_deployment_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.DeleteDirectory(TheDeploymentDirectory));
        }

        [Test]
        public void log_that_the_directory_was_deleted()
        {
            MockFor<ISimpleLogger>().AssertWasCalled(x => x.Log(InitializeCommand.DELETING_EXISTING_DIRECTORY, TheDeploymentDirectory));
        }

        [Test]
        public void should_create_the_deployment_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory));
        }

        [Test]
        public void should_create_the_recipes_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.RecipesFolder));
        }

        [Test]
        public void should_create_the_environments_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.EnvironmentsFolder));
        }

        [Test]
        public void should_create_the_bottles_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.BottlesFolder));
        }

        [Test]
        public void should_create_the_profiles_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.ProfilesFolder));
        }

        [Test]
        public void the_return_boolean_should_be_true_to_denote_success()
        {
            theReturnBooleanFlag.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_the_directory_does_not_already_exist_happy_path : InitializeCommandContext
    {
        protected override void theContextIs()
        {
            theDeploymentDirectoryDoesNotExist();
        }

        [Test]
        public void should_create_the_deployment_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory));
        }

        [Test]
        public void should_create_the_recipes_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.RecipesFolder));
        }

        [Test]
        public void should_create_the_environments_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.EnvironmentsFolder));
        }

        [Test]
        public void should_create_the_bottles_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.BottlesFolder));
        }

        [Test]
        public void should_create_the_profiles_directory()
        {
            MockFor<IFileSystem>().AssertWasCalled(x => x.CreateDirectory(TheDeploymentDirectory, ProfileFiles.ProfilesFolder));
        }

        [Test]
        public void the_return_boolean_should_be_true_to_denote_success()
        {
            theReturnBooleanFlag.ShouldBeTrue();
        }
    }


    [TestFixture]
    public class when_the_deployment_directory_already_exists_and_the_force_flag_is_false : InitializeCommandContext
    {
        protected override void theContextIs()
        {
            theDeploymentDirectoryExists();
            theInput.ForceFlag = false;
        }

        [Test]
        public void command_should_not_have_deleted_the_deployment_directory()
        {
            MockFor<IFileSystem>().AssertWasNotCalled(x => x.DeleteDirectory(theInput.RootDirectory(), ProfileFiles.DeploymentFolder));
        }

        [Test]
        public void should_not_have_made_any_new_folders()
        {
            MockFor<IFileSystem>().AssertWasNotCalled(x => x.CreateDirectory(null), x => x.IgnoreArguments());
        }

        [Test]
        public void logged_that_the_directory_was_not_re_created()
        {
            MockFor<ISimpleLogger>().AssertWasCalled(x => x.Log(InitializeCommand.DIRECTORY_ALREADY_EXISTS, TheDeploymentDirectory));
        }

        [Test]
        public void should_return_false_denoting_that_the_command_did_not_complete_successfully()
        {
            theReturnBooleanFlag.ShouldBeFalse();
        }
    }


    public abstract class InitializeCommandContext : InteractionContext<InitializeCommand>
    {
        protected InitializeInput theInput;
        protected string TheDeploymentDirectory;
        protected bool theReturnBooleanFlag;

        protected sealed override void beforeEach()
        {
            theInput = new InitializeInput(){
                DirectoryFlag = "Application1"
            };

            TheDeploymentDirectory = FileSystem.Combine(theInput.RootDirectory(), ProfileFiles.DeploymentFolder);

            theContextIs();

            theReturnBooleanFlag = ClassUnderTest.Initialize(theInput, MockFor<IFileSystem>(), MockFor<ISimpleLogger>());
        }

        protected abstract void theContextIs();



        protected void theDeploymentDirectoryExists()
        {
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theInput.RootDirectory(), ProfileFiles.DeploymentFolder))
                .Return(true);
        }

        protected void theDeploymentDirectoryDoesNotExist()
        {
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theInput.RootDirectory(), ProfileFiles.DeploymentFolder))
                .Return(false);
        }
    }
}