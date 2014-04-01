using System.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class ScriptTagTester
    {
        private FubuFile theFile;
        private Asset theAsset;

        public ScriptTagTester()
        {
            new FileSystem().WriteStringToFile("foo.js", "some stuff");

            theFile = new FubuFile("foo.js", ContentFolder.Application);
            theFile.RelativePath = "foo.js";
        }

        [SetUp]
        public void SetUp()
        {
            FubuMode.Mode("Production");
            theAsset = new Asset(theFile);
        }

        [Test]
        public void happy_path_prod_mode()
        {
            new ScriptTag(x => x, theAsset).ToString()
                .ShouldEqual("<script type=\"text/javascript\" src=\"foo.js\"></script>");
        }

        [Test]
        public void uses_the_default_url_if_asset_is_null()
        {
            new ScriptTag(x => x, null, defaultUrl: "http://server/foo.js")
                .ToString()
                .ShouldEqual("<script type=\"text/javascript\" src=\"http://server/foo.js\"></script>");
        }

        [Test]
        public void happy_path_dev_mode()
        {
            FubuMode.SetUpForDevelopmentMode();
            new ScriptTag(x => x, theAsset).ToString()
                .ShouldEqual("<script type=\"text/javascript\" src=\"foo.js?Etag={0}\"></script>".ToFormat(theFile.Etag()));
        }

        [Test]
        public void has_a_cdn_with_no_fallback()
        {
            theAsset.CdnUrl = "http://cdn.me.com/foo.js";

            new ScriptTag(x => x, theAsset).ToString()
                .ShouldEqual("<script type=\"text/javascript\" src=\"http://cdn.me.com/foo.js\"></script>");
        }

        [Test]
        public void has_a_cdn_and_fallback()
        {
            theAsset.CdnUrl = "http://cdn.me.com/foo.js";
            theAsset.FallbackTest = "foo == null";

            var tag = new ScriptTag(x => x, theAsset);

            tag.ToString().ShouldContain("<script>if (foo == null)");
        }
    }
}