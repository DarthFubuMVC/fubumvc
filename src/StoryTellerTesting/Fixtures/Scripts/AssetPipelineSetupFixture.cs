using System;
using System.Collections.Generic;
using Bottles;
using FubuCore.Util;
using StoryTeller.Engine;
using FubuCore;
using StoryTeller;

namespace IntegrationTesting.Fixtures.Scripts
{
    [Hidden]
    public class AssetPipelineSetupFixture : Fixture
    {
        private CommandRunner _runner;

        private static readonly string TemporaryPackagesFolder =
            AppDomain.CurrentDomain.BaseDirectory.AppendPath("packages");

        private string _packageDirectory;
        private FileSystem _fileSystem;


        public override void SetUp(ITestContext context)
        {
            _fileSystem = new FileSystem();
            _runner = context.Retrieve<CommandRunner>();
            removeAnyExistingPackages();
        }

        private void removeAnyExistingPackages()
        {
            _runner.RunFubu("link fubu-testing -cleanall");
            _runner.RunFubu("packages fubu-testing -cleanall -removeall");

            _fileSystem.DeleteDirectory(TemporaryPackagesFolder);
            _fileSystem.CreateDirectory(TemporaryPackagesFolder);
            _fileSystem.CleanDirectory(TemporaryPackagesFolder);
        }

        public override void TearDown()
        {
            _runner.RunFubu("restart fubu-testing");
        }

        [FormatAs("For package {packageName}")]
        public void ForPackage(string packageName)
        {
            _packageDirectory = TemporaryPackagesFolder.AppendPath(packageName);
            _fileSystem.CreateDirectory(_packageDirectory);

            var manifest = new PackageManifest{
                Role = "module",
                Name = packageName
            };

            _runner.RunFubu("link fubu-testing \"" + _packageDirectory + "\"");

            _fileSystem.PersistToFile(manifest, _packageDirectory, PackageManifest.FILE);
        }

        private readonly Cache<string, IList<string>> _contents = new Cache<string, IList<string>>(key => new List<string>());
        private string _currentFile;

        private void flushContents()
        {
            _contents.Each((file, contents) =>
            {
                var fileInPackage = _packageDirectory.AppendPath(file);

                _fileSystem.CreateDirectory(fileInPackage.ParentDirectory());

                _fileSystem.AlterFlatFile(fileInPackage, list => list.AddRange(contents));
            });

            _contents.ClearAll();
        }

        [Hidden]
        public void WriteLine(string File, string Contents)
        {
            _contents[File].Add(Contents);
        }

        public IGrammar WriteContents()
        {
            return this["WriteLine"]
                .AsTable("Asset File Contents")
                .After(flushContents);
        }


        [Hidden]
        [FormatAs("File {file}")]
        public void ForFile(string file)
        {
            _currentFile = file;
        }

        [Hidden]
        public void WriteFileLine(string Content)
        {
            _contents[_currentFile].Add(Content);
        }

        public IGrammar WriteFile()
        {
            return Paragraph("Write package file", x =>
            {
                x += this["ForFile"];
                x += this["WriteFileLine"].AsTable("has contents").LeafName("lines");
                x += flushContents;
            });
        }
    }
}