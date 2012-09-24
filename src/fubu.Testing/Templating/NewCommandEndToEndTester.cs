using System;
using System.IO;
using Bottles.Exploding;
using Bottles.Zipping;
using Fubu;
using Fubu.Templating;
using Fubu.Templating.Steps;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    [TestFixture, Explicit]
    public class NewCommandEndToEndTester
    {
        private NewCommand _command;
        private FileSystem _fileSystem;
        private ZipFileService _zipService;
        private NewCommandInput _commandInput;
        private string tmpDir;
        private string repoZip;
        private string solutionFile;
        private string solutionDir;
        private string oldContents;
        private bool _commandResult;
        private string newSolutionContents;

        [TestFixtureSetUp]
        public void before_all()
        {
            // setup
            _command = new NewCommand();
            _fileSystem = new FileSystem();
            _zipService = new ZipFileService(_fileSystem);
            _commandInput = new NewCommandInput();

            tmpDir = FileSystem.Combine("Templating", Guid.NewGuid().ToString());
            repoZip = FileSystem.Combine("Templating", "repo.zip");
            _zipService.ExtractTo(repoZip, tmpDir, ExplodeOptions.DeleteDestination);

            solutionFile = FileSystem.Combine("Templating", "sample", "myproject.txt");
            oldContents = _fileSystem.ReadStringFromFile(solutionFile);
            solutionDir = _fileSystem.GetDirectory(solutionFile);

            _commandInput.GitFlag = "file:///{0}".ToFormat(_fileSystem.GetFullPath(tmpDir).Replace("\\", "/"));
            _commandInput.ProjectName = "MyProject";
            _commandInput.SolutionFlag = solutionFile;
            _commandInput.OutputFlag = solutionDir;
            _commandInput.RakeFlag = "init.rb";

            _commandResult = _command.Execute(_commandInput);

            newSolutionContents = _fileSystem.ReadStringFromFile(solutionFile);
        }

        [TestFixtureTearDown]
        public void after_all()
        {
            new DirectoryInfo(tmpDir).SafeDelete();
            _fileSystem.DeleteDirectory("Templating", "sample", "MyProject");
            _fileSystem.DeleteDirectory("Templating", "sample", "MyProject.Tests");
            _fileSystem.WriteStringToFile(solutionFile, oldContents);
        }

        [Test]
        public void should_be_successful()
        {
            _commandResult.ShouldBeTrue();
        }

        [Test]
        public void should_create_projects()
        {
            _fileSystem
                .DirectoryExists("Templating", "sample", "MyProject")
                .ShouldBeTrue();

            _fileSystem
                .DirectoryExists("Templating", "sample", "MyProject.Tests")
                .ShouldBeTrue();

            _fileSystem
                .FileExists("Templating", "sample", "MyProject", "MyProject.csproj")
                .ShouldBeTrue();

            _fileSystem
                .FileExists("Templating", "sample", "MyProject.Tests", "MyProject.Tests.csproj")
                .ShouldBeTrue();
        }

        [Test]
        public void should_ignore_files_specified_in_fubuignore()
        {
            // these exist in the repo but shouldn't be copied over because they are specified in .fubuignore

            _fileSystem
                .FileExists("Templating", "sample", MoveContent.FubuIgnoreFile)
                .ShouldBeFalse();

            _fileSystem
                .FileExists("Templating", "sample", "ignored.txt")
                .ShouldBeFalse();
        }

        [Test]
        public void should_invoke_rake_callback()
        {
            // written to the file via the rake callback
            // FUBU_PROJECT_NAME gets loaded as a constant => fubu new FUBUPROJECTNAME
            splitSolutionContents()
                .ShouldContain("Hello, {0}".ToFormat(_commandInput.ProjectName));
        }

        [Test]
        public void should_invoke_auto_rake_callback()
        {
            // .fuburake file is conventionally picked up as a rake callback
            splitSolutionContents()
                .ShouldContain("From .fuburake");
        }

        [Test]
        public void should_not_copy_fuburake()
        {
            _fileSystem
                .FileExists("Templating", "sample", AutoRunFubuRake.FubuRakeFile)
                .ShouldBeFalse();
        }

        [Test]
        public void should_append_new_projects_to_existing_solution()
        {
            var lines = splitSolutionContents();
            var guid = _command.KeywordReplacer.Replace("GUID1");
            lines[2].ShouldEqual("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"MyProject\", \"MyProject\\MyProject.csproj\", \"{" + guid +  "}\"");
            lines[3].ShouldEqual("EndProject");
        }

        private string[] splitSolutionContents()
        {
            return ((SolutionFileService)_command.SolutionFileService).SplitSolution(newSolutionContents);
        }
    }
}