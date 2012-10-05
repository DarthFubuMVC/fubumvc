using System;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class finding_the_content_folder : InteractionContext<AssetFileGraphBuilder>
    {
        private string theDirectory;

        protected override void beforeEach()
        {
            theDirectory = "package1";
        }

        [Test]
        public void find_the_content_directory_if_lower_case_can_be_found()
        {
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("content"))).Return(true);
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("Content"))).Return(true);

            ClassUnderTest.FindContentFolder(theDirectory).ShouldEqual(theDirectory.AppendPath("content"));
        }

        [Test]
        public void find_the_Content_directory_if_lower_case_cannot_be_found()
        {
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("content"))).Return(false);
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("Content"))).Return(true);

            ClassUnderTest.FindContentFolder(theDirectory).ShouldEqual(theDirectory.AppendPath("Content"));
        }

        [Test]
        public void find_the_current_directory_should_be_null_if_neither_content_nor_Content_can_be_found()
        {
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("content"))).Return(false);
            MockFor<IFileSystem>().Stub(x => x.DirectoryExists(theDirectory.AppendPath("Content"))).Return(false);

            ClassUnderTest.FindContentFolder(theDirectory).ShouldBeNull();
        }
    }

    [TestFixture]
    public class when_the_content_folder_cannot_be_found_for_a_package : InteractionContext<AssetFileGraphBuilder>
    {
        private PackageAssetDirectory thePackageDirectory;

        protected override void beforeEach()
        {
            thePackageDirectory = new PackageAssetDirectory(){
                Directory = "package1",
                PackageName = "package1"
            };

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Stub(x => x.FindContentFolder(thePackageDirectory.Directory)).Return(null);

            ClassUnderTest.Expect(x => x.LoadFilesFromContentFolder(thePackageDirectory, null))
                .IgnoreArguments()
                .Repeat.Never();

            ClassUnderTest.LoadFiles(thePackageDirectory);
        }

        [Test]
        public void should_not_continue_loading()
        {
            ClassUnderTest.AssertWasNotCalled(x => x.LoadFilesFromContentFolder(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void logged_that_no_content_was_scanned()
        {
            var theMessage = AssetFileGraphBuilder.NoContentFoundForPackageAt.ToFormat(thePackageDirectory.Directory);
            MockFor<IPackageLog>().AssertWasCalled(x => x.Trace(theMessage));
        }
    }

    [TestFixture]
    public class when_the_content_folder_can_be_found_for_a_package : InteractionContext<AssetFileGraphBuilder>
    {
        private PackageAssetDirectory thePackageDirectory;
        private string theContentFolder;

        protected override void beforeEach()
        {
            thePackageDirectory = new PackageAssetDirectory()
            {
                Directory = "package1",
                PackageName = "package1"
            };

            theContentFolder = "content folder";

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Stub(x => x.FindContentFolder(thePackageDirectory.Directory)).Return(theContentFolder);

            ClassUnderTest.Expect(x => x.LoadFilesFromContentFolder(thePackageDirectory, theContentFolder));

            ClassUnderTest.LoadFiles(thePackageDirectory);
        }

        [Test]
        public void should_log_where_the_content_is_being_scanned_from()
        {
            var theMessage = AssetFileGraphBuilder.LoadingContentForPackageAt.ToFormat(theContentFolder);
            MockFor<IPackageLog>().AssertWasCalled(x => x.Trace(theMessage));
        }

        [Test]
        public void should_continue_to_scan_for_files()
        {
            ClassUnderTest.AssertWasCalled(x => x.LoadFilesFromContentFolder(thePackageDirectory, theContentFolder));
        }
    }

    
}