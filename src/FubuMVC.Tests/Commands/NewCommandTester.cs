using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Bottles.Exploding;
using Bottles.Zipping;
using Fubu;
using FubuCore;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class NewCommandTester : InteractionContext<NewCommand>
    {
        private NewCommandInput theInput;
        private IFileSystem fileSystem;
        private IZipFileService zipService;
        private IKeywordReplacer keywordReplacer;
        private IProcessFactory processFactory;

        protected override void beforeEach()
        {
            const string projectName = "TestProject";
            theInput = new NewCommandInput
                       {
                           ProjectName = projectName
                       };
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            zipService = MockRepository.GenerateStub<IZipFileService>();
            keywordReplacer = MockRepository.GenerateStub<IKeywordReplacer>();
            processFactory = MockRepository.GenerateStub<IProcessFactory>();

            fileSystem.Stub(x => x.FindFiles(Arg<string>.Is.Anything, Arg<FileSet>.Is.NotNull)).Return(new string[0]);

            ClassUnderTest.FileSystem = fileSystem;
            ClassUnderTest.ZipService = zipService;
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput);
        }

        [Test]
        public void should_unzip_the_template_specified_by_the_zipflag()
        {
            theInput.ZipFlag = Path.Combine(Environment.CurrentDirectory, "fubuTemplate.zip");
            var projectPath = Path.Combine(Environment.CurrentDirectory, theInput.ProjectName);

            ClassUnderTest.Unzip(theInput.ZipFlag, projectPath);

            zipService.AssertWasCalled(x => x.ExtractTo(theInput.ZipFlag, projectPath, ExplodeOptions.PreserveDestination));
        }

        [Test]
        public void should_move_the_directory_if_it_matches_a_template_keyword()
        {
            const string dirname = "FUBUPROJECTSHORTNAME";
            const string expectedDirname = "MyNewName";
            keywordReplacer.Stub(x => x.Replace(Arg<string>.Is.Anything)).Return(expectedDirname);
            NoChildDirectories();

            ClassUnderTest.KeywordReplacer = keywordReplacer;
            ClassUnderTest.ParseDirectory(dirname);

            fileSystem.AssertWasCalled(x => x.MoveDirectory(dirname, expectedDirname));
        }

        [Test]
        public void should_replace_keywords_in_file_content()
        {
            fileSystem.Stub(x => x.ReadTextFile(Arg<string>.Is.Anything, Arg<Action<string>>.Is.Anything));
            keywordReplacer.Stub(x => x.Replace(Arg<string>.Is.Anything)).Return("some replaced text");

            ClassUnderTest.KeywordReplacer = keywordReplacer;
            ClassUnderTest.ParseFile("somefile");
            fileSystem.AssertWasCalled(x => x.WriteStringToFile(Arg<string>.Is.Anything, Arg<string>.Is.Equal("some replaced text")));
        }

        [Test]
        public void should_replace_keywords_in_file_or_directory_name()
        {
            fileSystem.Stub(x => x.ReadTextFile(Arg<string>.Is.Anything, Arg<Action<string>>.Is.Anything));
            NoChildDirectories();
            keywordReplacer.Stub(x => x.Replace(Arg<string>.Is.Anything)).Return("newdir");

            ClassUnderTest.KeywordReplacer = keywordReplacer;
            ClassUnderTest.ParseDirectory("somedir");
            fileSystem.AssertWasCalled(x => x.MoveDirectory(Arg<string>.Is.Anything, Arg<string>.Is.Equal("newdir")));
        }

        [Test]
        public void should_create_a_git_process_with_the_git_flag()
        {
            NoChildDirectories();
            ClassUnderTest.ProcessFactory = processFactory;
            theInput.GitFlag = "https://github.com/DarthFubuMVC/fubumvc";

            var processInfo = new ProcessStartInfo();

            processFactory.Stub(x => x.Create(Arg<Action<ProcessStartInfo>>.Is.Anything))
                .Callback<Action<ProcessStartInfo>>(c =>
                {
                    c(processInfo);
                    return true;
                });

            execute();
            Assert.AreEqual("git", processInfo.FileName);
            Assert.True(processInfo.Arguments.Contains(theInput.GitFlag));
        }

        [Test]
        public void should_start_process_with_the_git_flag()
        {
            var mockProcess = MockFor<IProcess>();
            theInput.GitFlag = "https://github.com/DarthFubuMVC/fubumvc";
            NoChildDirectories();
            ClassUnderTest.ProcessFactory = processFactory;
            processFactory.Stub(x => x.Create(Arg<Action<ProcessStartInfo>>.Is.Anything))
                .Return(mockProcess);

            execute();
            mockProcess.AssertWasCalled(x => x.Start());
            mockProcess.AssertWasCalled(x => x.WaitForExit());
        }

        [Test]
        public void should_throw_when_exit_code_is_not_zero()
        {
            var mockProcess = MockFor<IProcess>();
            theInput.GitFlag = "https://github.com/DarthFubuMVC/fubumvc";
            NoChildDirectories();
            ClassUnderTest.ProcessFactory = processFactory;
            processFactory.Stub(x => x.Create(Arg<Action<ProcessStartInfo>>.Is.Anything))
                .Return(mockProcess);
            mockProcess.Stub(x => x.ExitCode).Return(-1);

            Assert.Throws<FubuException>(execute);
        }

        [Test]
        public void should_throw_when_target_directory_exists()
        {
            fileSystem.Stub(x => x.DirectoryExists(Arg<string>.Is.Anything))
                .Return(true);

            Assert.Throws<FubuException>(execute);
        }

        private void NoChildDirectories()
        {
            fileSystem.Stub(x => x.ChildDirectoriesFor(Arg<string>.Is.Anything)).Return(new string[0]);
        }

    }
}