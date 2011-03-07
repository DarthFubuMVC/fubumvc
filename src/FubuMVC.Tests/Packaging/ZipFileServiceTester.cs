using System.IO;
using System.Reflection;
using AssemblyPackage;
using FubuMVC.Core.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class ZipFileServiceTester
    {
        [SetUp]
        public void SetUp()
        {
            var assembly = typeof (AssemblyPackageMarker).Assembly;
            var stream = assembly.GetManifestResourceStream(typeof (AssemblyPackageMarker), "pak-data.zip");

            var service = new ZipFileService();
            service.ExtractTo("description of this", stream, "package-data");

            // These 3 files should be in the zip file embedded within the AssemblyPackage assembly
            File.Exists(Path.Combine("package-data", "1.txt")).ShouldBeTrue();
            File.Exists(Path.Combine("package-data", "2.txt")).ShouldBeTrue(); ;
            File.Exists(Path.Combine("package-data", "3.txt")).ShouldBeTrue(); ;
        }
    }
}