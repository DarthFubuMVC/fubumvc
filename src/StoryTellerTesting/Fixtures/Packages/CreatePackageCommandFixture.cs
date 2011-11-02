using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bottles.Exploding;
using Bottles.Zipping;
using StoryTeller;
using StoryTeller.Engine;
using FubuCore;
using System.Linq;

namespace IntegrationTesting.Fixtures.Packages
{
    public class CreatePackageCommandFixture : Fixture
    {
        private readonly CommandRunner _runner;
        private string _lastZipFile;

        public CreatePackageCommandFixture(CommandRunner runner)
        {
            Title = "Create Package Command";

            _runner = runner;
        }

        public override void TearDown()
        {
            if (File.Exists(_lastZipFile))
            {
                File.Delete(_lastZipFile);
            }
        }

        [FormatAs("init-pak {folder} {name}")]
        public void CreatePackageManifest(string folder, string name)
        {
            var command = "init-pak {0} {1}".ToFormat(folder, name);
            _runner.RunFubu(command);
        }

        [FormatAs("include-web-content {folder} -webforms")]
        public void AddWebFormsContent(string folder)
        {
            var command = "include-web-content {0} -webforms".ToFormat(folder);
            _runner.RunFubu(command);
        }

        [FormatAs("include-web-content {folder} -config")]
        public void AddConfigFiles(string folder)
        {
            var command = "include-web-content {0} -config".ToFormat(folder);
            _runner.RunFubu(command);
        }

        [FormatAs("create-pak {name} {zipFile}")]
        public void CreatePackage(string name, string zipFile)
        {
            _lastZipFile = zipFile;

            var command = "create-pak {0} {1} -f".ToFormat(name, zipFile);
            _runner.RunFubu(command);
        }

        public IGrammar CheckFilesInPackage()
        {
            return VerifyStringList(() => GetFilesInZip(_lastZipFile))
                .Titled("The contents of the package zip should be")
                .Grammar();
        }

        public static IEnumerable<string> GetFilesInZip(string zipFileName)
        {
            var tempPath = Path.GetTempPath();
            var zipDirectory = Path.Combine(tempPath, "zip-contents");

            Debug.WriteLine("Exploding the zip file {0} to {1}", zipFileName.ToFullPath(), zipDirectory);

            new ZipFileService(new FileSystem())
                .ExtractTo(zipFileName, zipDirectory, ExplodeOptions.DeleteDestination);

            return
                Directory.GetFiles(zipDirectory, "*", SearchOption.AllDirectories).Select(
                    x => x.PathRelativeTo(zipDirectory));
        } 
    }
}