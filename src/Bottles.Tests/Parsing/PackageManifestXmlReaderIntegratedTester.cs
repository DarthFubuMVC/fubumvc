using Bottles.Parsing;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Parsing
{
    [TestFixture]
    public class when_reading_a_serialized_package_manifest
    {
        private PackageManifest theOriginal;
        private PackageManifest theResultingManifest;

        [Test]
        public void read_a_package_manifest()
        {
            theOriginal = new PackageManifest{
                Name = "Han Solo",
                ContentFileSet = new FileSet(){
                    DeepSearch = true,
                    Exclude = "*.xml",
                    Include = "*.config"
                },
                DataFileSet = new FileSet(){
                    DeepSearch = true,
                    Exclude = "*.txt",
                    Include = "*.xml"
                }
            };


            theOriginal.AddAssembly("Fubu.Assem1");
            theOriginal.AddAssembly("Fubu.Assem2");
            theOriginal.AddAssembly("Fubu.Assem3");

            new FileSystem().WriteObjectToFile("manifest.xml", theOriginal);

            var reader = new PackageManifestXmlReader();
            theResultingManifest = reader.ReadFrom("manifest.xml");
        }

        [Test]
        public void should_read_the_name()
        {
            theResultingManifest.Name.ShouldEqual(theOriginal.Name);
        }

        [Test]
        public void should_read_all_the_assemblies()
        {
            theResultingManifest.Assemblies.ShouldHaveTheSameElementsAs(theOriginal.Assemblies);
        }

        [Test]
        public void should_read_the_data_file_set()
        {
            theResultingManifest.DataFileSet.ShouldEqual(theOriginal.DataFileSet);
        }

        [Test]
        public void should_read_the_content_file_set()
        {
            theResultingManifest.ContentFileSet.ShouldEqual(theOriginal.ContentFileSet);
        }
    }


    public class ApplicationManifest : PackageManifest{}


    [TestFixture]
    public class when_reading_a_serialized_application_manifest
    {
        private PackageManifest theOriginal;
        private PackageManifest theResultingManifest;

        [Test]
        public void read_a_package_manifest()
        {
            theOriginal = new ApplicationManifest
            {
                Name = "Han Solo",
                ContentFileSet = new FileSet()
                {
                    DeepSearch = true,
                    Exclude = "*.xml",
                    Include = "*.config"
                },
                DataFileSet = new FileSet()
                {
                    DeepSearch = true,
                    Exclude = "*.txt",
                    Include = "*.xml"
                }
            };


            theOriginal.AddAssembly("Fubu.Assem1");
            theOriginal.AddAssembly("Fubu.Assem2");
            theOriginal.AddAssembly("Fubu.Assem3");

            new FileSystem().WriteObjectToFile("manifest.xml", theOriginal);

            var reader = new PackageManifestXmlReader();
            theResultingManifest = reader.ReadFrom("manifest.xml");
        }

        [Test]
        public void should_read_the_name()
        {
            theResultingManifest.Name.ShouldEqual(theOriginal.Name);
        }

        [Test]
        public void should_read_all_the_assemblies()
        {
            theResultingManifest.Assemblies.ShouldHaveTheSameElementsAs(theOriginal.Assemblies);
        }

        [Test]
        public void should_read_the_data_file_set()
        {
            theResultingManifest.DataFileSet.ShouldEqual(theOriginal.DataFileSet);
        }

        [Test]
        public void should_read_the_content_file_set()
        {
            theResultingManifest.ContentFileSet.ShouldEqual(theOriginal.ContentFileSet);
        }
    }


}