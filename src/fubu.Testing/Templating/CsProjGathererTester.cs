using System.IO;
using System.Linq;
using Fubu.Templating;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class CsProjGathererTester
    {
        private CsProjGatherer _projGatherer;

        [SetUp]
        public void before_each()
        {
            _projGatherer = new CsProjGatherer(new FileSystem());
        }


        [Test]
        public void should_find_relative_paths()
        {
            var solutionFile = FileSystem.Combine("Templating", "data", "myproject.txt");
            var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionFile));
            var projectFile = FileSystem.Combine("Templating", "data", "example1", "example1.csproj");

            var relativePath = _projGatherer.FindRelativePath(solutionDir, projectFile);
            relativePath.ShouldEqual(@"example1\example1.csproj");
        }

        [Test]
        public void should_set_project_attributes()
        {
            var projectFile = FileSystem.Combine("Templating", "data", "example1", "example1.csproj");
            var project = new CsProj();

            _projGatherer.VisitProj(projectFile, project);

            project.ProjectGuid.ShouldEqual("GUID1");
            project.Name.ShouldEqual("FUBUPROJECTNAME");
        }

        [Test]
        public void should_find_projects()
        {
            var solutionFile = FileSystem.Combine("Templating", "data", "myproject.txt");
            var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionFile));

            var project = _projGatherer.GatherProjects(solutionDir).First();
            project.RelativePath.ShouldEqual(@"example1\example1.csproj");
            project.ProjectGuid.ShouldEqual("GUID1");
            project.Name.ShouldEqual("FUBUPROJECTNAME");
        }
    }
}