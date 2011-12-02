using System;
using FubuCore;
using FubuMVC.Core;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ApplicationSettingsTester
    {
        [Test]
        public void read_from_a_file_by_name()
        {
            new FileSystem().AlterFlatFile("proj1.application.config", list =>
            {
                list.Add("ApplicationSettings.PhysicalPath=source/proj1");
                list.Add("ApplicationSettings.RootUrl=http://localhost/proj1");
            });

            var settings = ApplicationSettings.ReadByName("proj1");
            settings.PhysicalPath.ShouldEqual("source/proj1");
            settings.RootUrl.ShouldEqual("http://localhost/proj1");
        }


        [Test]
        public void get_the_application_folder_with_a_file_but_no_physical_path()
        {
            var settings = new ApplicationSettings{
                PhysicalPath = null,
                ParentFolder = ".".ToFullPath()
            };

            settings.GetApplicationFolder().ShouldEqual(settings.ParentFolder);
        }

        [Test]
        public void get_the_application_folder_with_no_directory_and_no_folder_uses_the_appdomain()
        {
            new ApplicationSettings{
                ParentFolder = null,
                PhysicalPath = null
            }.GetApplicationFolder().ShouldEqual(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void get_the_application_folder_when_the_physical_path_is_absolute()
        {
            new ApplicationSettings{
                ParentFolder = ".".ToFullPath(),
                PhysicalPath = "app1".ToFullPath()
            }.GetApplicationFolder().ShouldEqual("app1".ToFullPath());
        }

        [Test]
        public void get_the_application_folder_when_the_physical_path_is_relative()
        {
            var settings = new ApplicationSettings{
                ParentFolder = ".".ToFullPath(),
                PhysicalPath = "app1"
            };

            settings.GetApplicationFolder().ShouldEqual(settings.ParentFolder.AppendPath(settings.PhysicalPath));


        }


    }
}