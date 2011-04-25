using System;
using Bottles.Creation;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace Bottles.Tests.Creation
{
    [TestFixture]
    public class PackageManifestWriterTester : InteractionContext<PackageManifestWriter<PackageManifest>>
    {
        private const string theFileName = ".manifest";

        [Test]
        public void read_from_when_the_manifest_does_not_exist()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theFileName)).Return(false);
            var action = MockRepository.GenerateMock<Action<PackageManifest>>();

            ClassUnderTest.ReadFrom(theFileName, action);

            action.AssertWasCalled(x => x.Invoke(ClassUnderTest.Manifest));
        }

        [Test]
        public void read_when_the_manifest_does_exist()
        {
            var theManifest = new PackageManifest();
            MockFor<IFileSystem>().Stub(x => x.FileExists(theFileName)).Return(true);

            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<PackageManifest>(theFileName)).Return(theManifest);

            ClassUnderTest.ReadFrom(theFileName, m => Assert.Fail("should not be called"));

            ClassUnderTest.Manifest.ShouldBeTheSameAs(theManifest);
        }
    }
}