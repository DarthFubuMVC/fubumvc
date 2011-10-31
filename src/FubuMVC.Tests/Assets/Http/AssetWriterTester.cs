using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class AssetWriterTester : InteractionContext<AssetWriter>
    {
        private string theEtag;
        private AssetFile[] theFiles;
        private AssetPath theAssetPath;
        private HttpHeaderValues theHeaders;

        protected override void beforeEach()
        {
            theEtag = "12345";

            theFiles = new AssetFile[]{new AssetFile("a"), new AssetFile("b")};

            theAssetPath = new AssetPath("scripts/something"){
                ResourceHash = Guid.NewGuid().ToString()
            };

            MockFor<IContentWriter>().Expect(x => x.Write(theAssetPath))
                .Return(theFiles);

            MockFor<IETagGenerator<IEnumerable<AssetFile>>>()
                .Stub(x => x.Create(theFiles))
                .Return(theEtag);

            theHeaders = ClassUnderTest.Write(theAssetPath);
        }

        [Test]
        public void should_write_all_the_content()
        {
            VerifyCallsFor<IContentWriter>();
        }

        [Test]
        public void should_apply_the_etag_from_all_the_files_to_the_returned_value()
        {
            theHeaders[HttpResponseHeaders.ETag].ShouldEqual("12345");
        }

        [Test]
        public void should_have_linked_all_the_files_to_a_resource_hash()
        {
            MockFor<IAssetContentCache>().AssertWasCalled(x => x.LinkFilesToResource(theAssetPath.ResourceHash, theFiles));
        }
    }
}