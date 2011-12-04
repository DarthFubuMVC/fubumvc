using Fubu.Applications;
using FubuCore;
using FubuMVC.Core;
using KayakTestApplication;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuMVC.Tests.Applications
{
    [TestFixture]
    public class KayakCommandTester
    {
        private FileSystem theFileSystem;
        private string aSpecificLocation;
        private ApplicationSettings theOriginalSettings;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = new FileSystem();
        
            theFileSystem.DeleteDirectory("fake-app");
            theFileSystem.CreateDirectory("fake-app");

            theFileSystem.FindFiles(".".ToFullPath(), ApplicationSettings.FileSearch())
                .Each(x => theFileSystem.DeleteFile(x));

            theOriginalSettings = ApplicationSettings.For<KayakApplication>();
            theOriginalSettings.ParentFolder = "fake-app".ToFullPath();
            theOriginalSettings.Write();

            aSpecificLocation = theOriginalSettings.GetFileName();


        }

        [Test]
        public void find_settings_for_a_specific_app_settings_file_that_exists()
        {
            var input = new KayakInput{
                Location = aSpecificLocation
            };

            var settings = KayakCommand.FindSettings(input);

            settings.ApplicationSourceName.ShouldEqual(theOriginalSettings.ApplicationSourceName);
            settings.Name.ShouldEqual(theOriginalSettings.Name);
        }


        [Test]
        public void find_settings_for_a_folder_if_there_is_only_one_settings_file()
        {
            var input = new KayakInput
            {
                Location = theOriginalSettings.ParentFolder
            };

            var settings = KayakCommand.FindSettings(input);

            settings.ApplicationSourceName.ShouldEqual(theOriginalSettings.ApplicationSourceName);
            settings.Name.ShouldEqual(theOriginalSettings.Name);
        }

        [Test]
        public void undeterministic_application_with_a_directory_and_multiple_settings()
        {
            var additionalSettings = ApplicationSettings.For<KayakApplication>();
            additionalSettings.ParentFolder = "fake-app".ToFullPath();
            additionalSettings.Name = "SomethingElse";
            
            additionalSettings.Write();

            var input = new KayakInput
            {
                Location = theOriginalSettings.ParentFolder
            };

            KayakCommand.FindSettings(input).ShouldBeNull();
        }

        [Test]
        public void no_application_files_exist_so_try_settings_with_just_the_physical_path()
        {
            theFileSystem.DeleteFile(theOriginalSettings.GetFileName());

            var input = new KayakInput
            {
                Location = theOriginalSettings.ParentFolder
            };

            var settings = KayakCommand.FindSettings(input);
            settings.ApplicationSourceName.ShouldBeNull();
            settings.Name.ShouldBeNull();
            settings.PhysicalPath.ShouldEqual(input.Location);
            settings.ParentFolder.ShouldEqual(input.Location);

        }

        [Test]
        public void location_is_app_name()
        {
            var input = new KayakInput
            {
                Location = theOriginalSettings.Name
            };

            var settings = KayakCommand.FindSettings(input);

            settings.ApplicationSourceName.ShouldEqual(theOriginalSettings.ApplicationSourceName);
            settings.Name.ShouldEqual(theOriginalSettings.Name);
        }

        [Test]
        public void location_is_null_try_to_use_the_current_directory()
        {
            theFileSystem.FindFiles(".".ToFullPath(), ApplicationSettings.FileSearch())
                .Each(x => theFileSystem.DeleteFile(x));

            var settings = KayakCommand.FindSettings(new KayakInput{
                Location = null
            });
            settings.PhysicalPath.ShouldEqual(".".ToFullPath());
            settings.ParentFolder.ShouldEqual(".".ToFullPath());
        }
    }
}