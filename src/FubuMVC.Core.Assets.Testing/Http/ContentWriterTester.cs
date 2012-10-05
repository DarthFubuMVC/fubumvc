using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using FubuCore;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class when_writing_an_image : InteractionContext<ContentWriter>
    {
        private AssetPath theAssetPath;
        private AssetFile theFile;
        private IEnumerable<AssetFile> theReturnedFiles;

        protected override void beforeEach()
        {
            theAssetPath = new AssetPath("images/icon.gif");

            // Precondition here
            theAssetPath.IsBinary().ShouldBeTrue();

            theFile = new AssetFile(theAssetPath.ToFullName()){
                FullPath = theAssetPath.ToFullName().ToFullPath()
            };

            MockFor<IAssetFileGraph>().Stub(x => x.Find(theAssetPath))
                .Return(theFile);

            theReturnedFiles = ClassUnderTest.Write(theAssetPath);
        }

        [Test]
        public void should_have_returned_the_single_file()
        {
            theReturnedFiles.Single().ShouldEqual(theFile);
        }

        [Test]
        public void should_have_written_the_actual_image_file_to_the_output_writer()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteFile(MimeType.Gif, theFile.FullPath, null));
        }
    }

    [TestFixture]
    public class when_writing_textual_output : InteractionContext<ContentWriter>
    {
        private AssetFile[] theFiles;
        private IEnumerable<AssetFile> theReturnedFiles;
        private const string theContent = "blah blah blah";

        protected override void beforeEach()
        {
            var assetPath = new AssetPath("scripts/combo1.js");
            assetPath.IsBinary().ShouldBeFalse();

            theFiles = new AssetFile[]{
                new AssetFile("script1.js"){FullPath = "1.js"},
                new AssetFile("script2.js"){FullPath = "2.js"},
                new AssetFile("script3.js"){FullPath = "3.js"},
                new AssetFile("script4.js"){FullPath = "4.js"},
            };

            MockFor<IContentPlanCache>().Stub(x => x.SourceFor(assetPath))
                .Return(MockFor<IContentSource>());

            MockFor<IContentSource>().Expect(x => x.GetContent(MockFor<IContentPipeline>()))
                .Return(theContent);

            MockFor<IContentSource>().Stub(x => x.Files).Return(theFiles);


            theReturnedFiles = ClassUnderTest.Write(assetPath);
        }

        [Test]
        public void should_execute_the_content_plan()
        {
            VerifyCallsFor<IContentSource>();
        }

        [Test]
        public void returns_all_the_files_from_the_content_plan_source()
        {
            theReturnedFiles.ShouldHaveTheSameElementsAs(theFiles);
        }

        [Test]
        public void should_write_out_the_contents()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MimeType.Javascript, theContent));
        }


    }

    




}