using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fubu.Generation;
using FubuCore;
using FubuCore.CommandLine;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Engine;

namespace fubu.Testing.Fixtures
{
    public class TemplatingFixture : Fixture
    {
        public static FileSystem FileSystem = new FileSystem();
        private string _folder;
        private string _original;

        private string _processPath;
        private string _root;

        public override void SetUp(ITestContext context)
        {
            RemoteOperations.Enabled = false;

            _root = AppDomain.CurrentDomain.BaseDirectory.AppendPath("Templating").ToFullPath();

            FileSystem.DeleteDirectory(_root);
            FileSystem.CreateDirectory(_root);


            string compile = AppDomain.CurrentDomain.BaseDirectory.ToLower().EndsWith("debug")
                ? "debug"
                : "release";

            _processPath =
                AppDomain.CurrentDomain.BaseDirectory.ParentDirectory()
                    .ParentDirectory()
                    .ParentDirectory()
                    .AppendPath("Fubu", "bin", compile, "Fubu.exe");

            _original = Environment.CurrentDirectory;

            Environment.CurrentDirectory = _root;

        }

        public override void TearDown()
        {
            Environment.CurrentDirectory = _original;
        }

        [FormatAs("Run fubu {command}")]
        public void Execute(string command)
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(typeof (NewCommand).Assembly);

            factory.BuildRun(command).Execute();
        }

        [FormatAs("For folder {folder}")]
        public void ForFolder(string folder)
        {
            _folder = folder;

            Environment.CurrentDirectory = Environment.CurrentDirectory.AppendPath(folder);
        }

        [FormatAs("The rake script can run successfully")]
        public bool RakeSucceeds()
        {
            string workingDirectory = _root.AppendPath(_folder);


            var rake = new ProcessStartInfo
            {
                UseShellExecute = !FubuCore.Platform.IsUnix (),
                FileName = "rake",
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            Process process = Process.Start(rake);
            process.WaitForExit();

            StoryTellerAssert.Fail(process.ExitCode != 0, "Rake failed at directory {0}!".ToFormat(workingDirectory));

            return true;
        }

        [ExposeAsTable("Files should exist")]
        public bool FileExists(string Name)
        {
            return FileSystem.FileExists(_root, Name);
        }

        public IGrammar AllTheFilesShouldBe()
        {
            return VerifyStringList(allFiles)
                .Titled("All the files generated are")
                .Grammar();
        }

        private IEnumerable<string> allFiles()
        {
            string path = _root.AppendPath(_folder);

            FileSet searchSpecification = FileSet.Everything();
            searchSpecification.Exclude = "logs/*.*;instructions.txt;*-TestResults.xml;fubu.templates.config";

            return
                FileSystem.FindFiles(path, searchSpecification)
                    .Select(x => x.PathRelativeTo(path).Replace("\\", "/"))
                    .Where(x => !x.StartsWith("src/packages"))
                    .Where(x => !x.Contains("/bin/"))
                    .Where(x => !x.Contains("/obj/"))
                    .Where(x => !x.Contains("fubu.templates.config"))
                    .Where(x => !x.Contains(".bottle-alias"))
                    .Where(x => !x.Contains("TestResults.xml"));

        }

        [FormatAs("File {File} should contain {Contents}")]
        public bool FileContains(string File, string Contents)
        {
            string contents = FileSystem.ReadStringFromFile(_root.AppendPath(File));

            StoryTellerAssert.Fail(!contents.Contains(Contents), contents);

            return true;
        }

        [FormatAs("File {File} should not contain {Contents}")]
        public bool FileDoesNotContain(string File, string Contents)
        {
            string contents = FileSystem.ReadStringFromFile(_root.AppendPath(File));

            StoryTellerAssert.Fail(contents.Contains(Contents), contents);

            return true;
        }
    }
}