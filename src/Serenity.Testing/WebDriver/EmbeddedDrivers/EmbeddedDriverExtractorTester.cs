using System;
using System.IO;
using FubuCore;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;
using Serenity.WebDriver.EmbeddedDrivers;

namespace Serenity.Testing.WebDriver.EmbeddedDrivers
{
    [TestFixture(typeof(ChromeEmbeddedDriver))]
    [TestFixture(typeof(PhantomEmbeddedDriver))]
    public class EmbeddedDriverExtractorTester<TEmbeddedDriver> : InteractionContext<EmbeddedDriverExtractor<TEmbeddedDriver>> where TEmbeddedDriver : IEmbeddedDriver, new()
    {
        private readonly string WorkingDir = AppDomain.CurrentDomain.BaseDirectory;
        private readonly IEmbeddedDriver _embeddedDriver = new TEmbeddedDriver();


        [Test]
        public void VersionDeclarationFilePathInWorkingDirectory()
        {
            ClassUnderTest.PathToVersionDeclarationFile.ShouldBe(Path.Combine(WorkingDir, _embeddedDriver.ExtractedFileName + ".version"));
        }

        [Test]
        public void DriverPathIsInWorkingDirectoty()
        {
            ClassUnderTest.PathToDriver.ShouldBe(Path.Combine(WorkingDir, _embeddedDriver.ExtractedFileName));
        }

        [Test]
        public void ShouldExtractIfNoVersion()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToVersionDeclarationFile)).Return(false);
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToDriver)).Return(true);

            ClassUnderTest.ShouldExtract().ShouldBeTrue();
        }

        [Test]
        public void ShouldExtractIfNoDriver()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToVersionDeclarationFile)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToDriver)).Return(false);

            ClassUnderTest.ShouldExtract().ShouldBeTrue();
        }

        [Test]
        public void ShouldExtractIfExtractedVersionIsLessThenEmbedded()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToVersionDeclarationFile)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToDriver)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.ReadStringFromFile(ClassUnderTest.PathToVersionDeclarationFile)).Return("1.0");

            ClassUnderTest.ShouldExtract().ShouldBeTrue();
        }

        [Test]
        public void ShouldNotExtractIfExtractedVersionIsEqualToEmbedded()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToVersionDeclarationFile)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToDriver)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.ReadStringFromFile(ClassUnderTest.PathToVersionDeclarationFile)).Return(_embeddedDriver.Version.ToString());

            ClassUnderTest.ShouldExtract().ShouldBeFalse();
        }

        [Test]
        public void ShouldNotExtractIfExtractedVersionIsGreaterThenEmbedded()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToVersionDeclarationFile)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.FileExists(ClassUnderTest.PathToDriver)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.ReadStringFromFile(ClassUnderTest.PathToVersionDeclarationFile)).Return("99.0");

            ClassUnderTest.ShouldExtract().ShouldBeFalse();
        }

        [Test]
        public void ExtractingDeletesOldDriver()
        {
            ClassUnderTest.Extract();
            MockFor<IFileSystem>().AssertWasCalled(x => x.DeleteFile(ClassUnderTest.PathToDriver));
        }

        [Test]
        public void ExtractingDeletesOldVersion()
        {
            ClassUnderTest.Extract();
            MockFor<IFileSystem>().AssertWasCalled(x => x.DeleteFile(ClassUnderTest.PathToVersionDeclarationFile));
        }

        [Test]
        public void ExtractingWritesDriverStreamToFile()
        {
            MockFor<IFileSystem>().Expect(x => x.WriteStreamToFile(
                Arg<string>.Matches(path => path == ClassUnderTest.PathToDriver),
                Arg<Stream>.Matches(stream => stream.Length > 0)));
            ClassUnderTest.Extract();

            MockFor<IFileSystem>().VerifyAllExpectations();
        }

        [Test]
        public void ExtractingWritesVersionToFile()
        {
            ClassUnderTest.Extract();
            MockFor<IFileSystem>().AssertWasCalled(x => x.WriteStringToFile(
                ClassUnderTest.PathToVersionDeclarationFile,
                _embeddedDriver.Version.ToString()));
        }


    }
}