using System;
using System.IO;
using Bottles.Zipping;
using FubuCore;
using FubuTestingSupport;
using Ionic.Zip;
using NUnit.Framework;

namespace Bottles.Tests.Zipping
{
    [TestFixture]
    public class ZipFileServiceTester
    {
        [Test]
        public void read_version_out_of_a_zip_file()
        {
            var versionFile = Path.Combine(Path.GetTempPath(), BottleFiles.VersionFile);
            var guid = Guid.NewGuid();
            new FileSystem().WriteStringToFile(versionFile, guid.ToString());

            if (File.Exists("zip1.zip"))
            {
                File.Delete("zip1.zip");
            }

            using (var zip1 = new ZipFile("zip1.zip"))
            {
                zip1.AddFile(versionFile, "");
                zip1.Save();
            }

            var service = new ZipFileService(new FileSystem());

            service.GetVersion("zip1.zip").ShouldEqual(guid);
        }

        [Test]
        public void read_a_package_manifest()
        {
            var manifest = new PackageManifest{
                Role = "application",
                Name = "something"
            };

            new FileSystem().WriteObjectToFile(PackageManifest.FILE, manifest);

            if (File.Exists("zip1.zip"))
            {
                File.Delete("zip1.zip");
            }

            using (var zip1 = new ZipFile("zip1.zip"))
            {
                zip1.AddFile(PackageManifest.FILE, "");
                zip1.Save();
            }

            var service = new ZipFileService(new FileSystem());
            var manifest2 = service.GetPackageManifest("zip1.zip");

            manifest2.Name.ShouldEqual(manifest.Name);
            manifest2.Role.ShouldEqual(manifest.Role);
        }

        [Test]
        public void create_test_zip_to_a_nonexistent_path()
        {
            var fileSystem = new FileSystem();
            fileSystem.DeleteDirectory(".\\nonexist");

            fileSystem.FileExists(".\\nonexist\\silly.zip").ShouldBeFalse();

            fileSystem.WriteStringToFile(".\\bob.txt","hi");
            var service = new ZipFileService(fileSystem);
            service.CreateZipFile(".\\nonexist\\silly.zip", f=>
            {
                f.AddFile(".\\bob.txt","");
            });

            fileSystem.FileExists(".\\nonexist\\silly.zip").ShouldBeTrue();
        }
    }
}