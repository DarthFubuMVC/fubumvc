using System;
using Fubu;
using FubuCore;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class ManifestCommandTester : InteractionContext<ManifestCommand>
    {
        private ManifestInput theInput;
        private ApplicationManifest theManifest;

        protected override void beforeEach()
        {
            theInput = new ManifestInput(){
                AppFolder = "some folder"
            };

            theManifest = new ApplicationManifest();

        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());
        }

        [Test]
        public void set_assembly_on_manifest_if_it_is_given()
        {
            theInput.AssemblyFlag = "some assembly";
            ClassUnderTest.ApplyChanges(theInput, theManifest);

            theManifest.EnvironmentAssembly.ShouldEqual("some assembly");
        }

        [Test]
        public void should_not_overwrite_assembly_on_manifest_if_the_input_value_is_empty()
        {
            theInput.AssemblyFlag = null;
            theManifest.EnvironmentAssembly = "start";

            ClassUnderTest.ApplyChanges(theInput, theManifest);

            theManifest.EnvironmentAssembly.ShouldEqual("start");
        }

        [Test]
        public void set_environment_class_if_it_is_given()
        {
            theInput.EnvironmentClassNameFlag = "class name";
            ClassUnderTest.ApplyChanges(theInput, theManifest);

            theManifest.EnvironmentClassName.ShouldEqual("class name");
        }

        [Test]
        public void do_not_overwrite_the_environment_class_if_it_is_not_given_in_the_input()
        {
            theInput.EnvironmentClassNameFlag = null;
            theManifest.EnvironmentClassName = "something";

            ClassUnderTest.ApplyChanges(theInput, theManifest);

            theManifest.EnvironmentClassName.ShouldEqual("something");
        }



        [Test]
        public void execute_when_the_folder_cannot_be_found_and_create_is_false()
        {
            theInput.CreateFlag = false;

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.WriteManifestCannotBeFound(theInput.AppFolder));

            theManifestFileDoesNotExist();
            
            execute();
        
            ClassUnderTest.VerifyAllExpectations();
        }

        private void theManifestFileDoesNotExist()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, ApplicationManifest.FILE)).Return(false);
        }

        private void theManifestFileExists()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, ApplicationManifest.FILE)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<ApplicationManifest>(theInput.AppFolder, ApplicationManifest.FILE)).Return(theManifest);
        }

        [Test]
        public void execute_with_just_the_folder_and_the_file_does_exist()
        {
            theManifestFileExists();
            theInput.CreateFlag = false;
            

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Expect(x => x.WriteManifest(theInput, theManifest));
            ClassUnderTest.Expect(x => x.ApplyChanges(theInput, theManifest)).Return(false);

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }



        [Test]
        public void the_file_exists_create_flag_is_true_but_not_force()
        {
            theInput.CreateFlag = true;
            theInput.ForceFlag = false;

            theManifestFileExists();

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Expect(x => x.WriteCannotOverwriteFileWithoutForce(theInput.AppFolder));

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void the_file_exists_create_is_true_and_force_is_true()
        {
            theInput.CreateFlag = true;
            theInput.ForceFlag = true;
            theManifestFileExists();

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Expect(x => x.CreateManifest(MockFor<IFileSystem>(), theInput));

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void the_file_exists_and_the_open_flag_is_true()
        {
            theManifestFileExists();
            theInput.OpenFlag = true;

            execute();

            MockFor<IFileSystem>().AssertWasCalled(x => x.LaunchEditor(theInput.AppFolder, ApplicationManifest.FILE));
        }
    }
}