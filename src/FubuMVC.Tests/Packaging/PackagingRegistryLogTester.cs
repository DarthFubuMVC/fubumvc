using FubuMVC.Core.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackagingRegistryLogTester
    {
        [Test]
        public void write_trace()
        {
            var log = new PackageRegistryLog();
            log.FullTraceText().ShouldBeEmpty();

            log.Trace("stuff");
            log.Trace("other");
            log.Trace("new");

            log.FullTraceText().ShouldContain("stuff");
            log.FullTraceText().ShouldContain("other");
            log.FullTraceText().ShouldContain("new");
        }

        [Test]
        public void find_children()
        {
            var log = new PackageRegistryLog();

            var loader1 = new StubPackageLoader();
            var loader2 = new StubPackageLoader();
            var loader3 = new StubPackageLoader();

            var package1 = new StubPackage("1");
            var package2 = new StubPackage("2");
            var package3 = new StubPackage("3");
        
            log.AddChild(loader1, loader2, loader3, package1, package2, package3);

            log.FindChildren<IPackageLoader>().ShouldHaveTheSameElementsAs(loader1, loader2, loader3);

            log.FindChildren<IPackageInfo>().ShouldHaveTheSameElementsAs(package1, package2, package3);
        }
    }
}