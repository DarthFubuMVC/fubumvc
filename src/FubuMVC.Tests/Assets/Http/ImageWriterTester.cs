using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class ImageWriterTester : InteractionContext<ImageWriter>
    {
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("icon.gif", AssetFolder.images){
                FullPath = "some path"
            };

            MockFor<IAssetPipeline>()
                .Stub(x => x.Find(theFile.Name))
                .Return(theFile);
        
            ClassUnderTest.WriteImageToOutput(theFile.Name);
        }

        [Test]
        public void should_write_the_image_file_to_output_with_the_gif_mimetype()
        {
            // pre-condition
            theFile.MimeType.ShouldEqual(MimeType.Gif);

            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteFile(MimeType.Gif.Value, theFile.FullPath, null));
        }

        [Test]
        public void should_set_up_caching_by_the_image_file()
        {
            MockFor<IResponseCaching>().AssertWasCalled(x => x.CacheRequestAgainstFileChanges(theFile.FullPath));
        }
    }
}