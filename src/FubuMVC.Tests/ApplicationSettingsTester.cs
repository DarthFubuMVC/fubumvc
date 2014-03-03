using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using NUnit.Framework;
using FubuTestingSupport;
using StructureMap;

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

        [Test]
        public void generate_default_settings_for_an_application()
        {
            var settings1 = ApplicationSettings.For<KayakApplication>();
            settings1.Name.ShouldEqual("Kayak");
            settings1.Port.ShouldEqual(5500);
            settings1.RootUrl.ShouldEqual("http://localhost/kayak");
            settings1.ApplicationSourceName.ShouldEqual(typeof (KayakApplication).AssemblyQualifiedName);
        }

        [Test]
        public void write_and_read()
        {
            var settings1 = ApplicationSettings.For<KayakApplication>();
            settings1.Port = 5501;
            
            settings1.Write();

            var settings2 = ApplicationSettings.ReadByName(settings1.Name);
            settings1.Name.ShouldEqual(settings2.Name);
            settings1.Port.ShouldEqual(settings2.Port);
            settings1.RootUrl.ShouldEqual(settings2.RootUrl);
            settings1.ApplicationSourceName.ShouldEqual(settings2.ApplicationSourceName);
        }


    }

    public class KayakApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .For<KayakRegistry>()
                .StructureMap(new Container());
        }
    }

    public class KayakRegistry : FubuRegistry
    {
        public KayakRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();
        }
    }

    public class NameModel
    {
        public string Name { get; set; }
    }

    public class SayHelloController
    {
        public string Hello()
        {
            return "Hello, it's " + DateTime.Now;
        }

        public NameModel get_say_Name(NameModel model)
        {
            return model;
        }

        public IDictionary<string, object> post_name(NameModel model)
        {
            return new Dictionary<string, object> { { "name", model.Name } };
        }
    }
}