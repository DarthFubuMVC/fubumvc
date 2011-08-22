using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class MissingAssetTagSubjectTester
    {
        [Test]
        public void creating_missing_asset_sets_the_mimetype()
        {
            var subject = new MissingAssetTagSubject("file.js");
            subject.MimeType.ShouldEqual(MimeType.Javascript);


            new MissingAssetTagSubject("file.css")
                .MimeType.ShouldEqual(MimeType.Css);
        }
    }
}