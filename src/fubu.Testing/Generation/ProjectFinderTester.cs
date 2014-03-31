using System;
using Fubu.Generation;
using FubuCore;
using FubuCsProjFile;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class ProjectFinderTester
    {
        FileSystem fileSystem = new FileSystem();

        [Test]
        public void find_shallow()
        {
            fileSystem.DeleteDirectory("shallow");
            fileSystem.CreateDirectory("shallow");

            CsProjFile.CreateAtSolutionDirectory("Foo", "shallow").Save();

            var location = ProjectFinder.DetermineLocation("shallow".AppendPath("Foo"));
            location.Project.AssemblyName.ShouldEqual("Foo");
            location.Namespace.ShouldEqual("Foo");
            location.RelativePath.ShouldBeEmpty();
        }

        [Test]
        public void find_deep_1()
        {
            fileSystem.DeleteDirectory("deep1");
            fileSystem.CreateDirectory("deep1");

            CsProjFile.CreateAtSolutionDirectory("Foo", "deep1").Save();

            fileSystem.CreateDirectory("deep1","Foo", "A");

            var location = ProjectFinder.DetermineLocation("deep1".AppendPath("Foo", "A"));
            location.Project.AssemblyName.ShouldEqual("Foo");
            location.Namespace.ShouldEqual("Foo.A");
            location.RelativePath.ShouldEqual("A");

            
        }

        [Test]
        public void sets_the_CurrentFolder()
        {
            fileSystem.DeleteDirectory("deep1");
            fileSystem.CreateDirectory("deep1");

            CsProjFile.CreateAtSolutionDirectory("Foo", "deep1").Save();

            fileSystem.CreateDirectory("deep1", "Foo", "A");

            var location = ProjectFinder.DetermineLocation("deep1".AppendPath("Foo", "A"));

            location.CurrentFolder.ToFullPath().ShouldEqual(Environment.CurrentDirectory.ToFullPath().AppendPath("deep1", "Foo", "A"));
        }


        [Test]
        public void find_deep_2()
        {
            fileSystem.DeleteDirectory("deep2");
            fileSystem.CreateDirectory("deep2");

            CsProjFile.CreateAtSolutionDirectory("Foo", "deep2").Save();

            fileSystem.CreateDirectory("deep2", "Foo", "A", "B");

            var location = ProjectFinder.DetermineLocation("deep2".AppendPath("Foo", "A", "B"));
            location.Project.AssemblyName.ShouldEqual("Foo");
            location.Namespace.ShouldEqual("Foo.A.B");
            location.RelativePath.ShouldEqual("A\\B");
        }
    }
}