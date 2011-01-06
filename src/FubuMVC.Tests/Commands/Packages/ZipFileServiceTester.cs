using System;
using System.IO;
using Fubu.Packages;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using Ionic.Zip;
using NUnit.Framework;

namespace FubuMVC.Tests.Commands.Packages
{
    [TestFixture]
    public class ZipFileServiceTester
    {
        [Test]
        public void read_version_out_of_a_zip_file()
        {
            var versionFile = Path.Combine(Path.GetTempPath(), FubuMvcPackages.VersionFile);
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

            var service = new ZipFileService();

            service.GetVersion("zip1.zip").ShouldEqual(guid);
        }
    }
}