﻿using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime.Files;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Assets
{
    
    public class ScriptTagTester
    {
        private readonly FubuFile theFile;
        private Asset theAsset;

        public ScriptTagTester()
        {
            new FileSystem().WriteStringToFile("foo.js", "some stuff");

            theFile = new FubuFile("foo.js");
            theFile.RelativePath = "foo.js";

            theAsset = new Asset(theFile);
        }


        [Fact]
        public void happy_path_prod_mode()
        {
            new ScriptTag(null, x => x, theAsset).ToString()
                .ShouldBe("<script type=\"text/javascript\" src=\"foo.js\"></script>");
        }

        [Fact]
        public void uses_the_default_url_if_asset_is_null()
        {
            new ScriptTag(null, x => x, null, "http://server/foo.js")
                .ToString()
                .ShouldBe("<script type=\"text/javascript\" src=\"http://server/foo.js\"></script>");
        }

        [Fact]
        public void happy_path_dev_mode()
        {
            new ScriptTag("development", x => x, theAsset).ToString()
                .ShouldBe("<script type=\"text/javascript\" src=\"foo.js?Etag={0}\"></script>".ToFormat(theFile.Etag()));
        }

        [Fact]
        public void has_a_cdn_with_no_fallback()
        {
            theAsset.CdnUrl = "http://cdn.me.com/foo.js";

            new ScriptTag(null, x => x, theAsset).ToString()
                .ShouldBe("<script type=\"text/javascript\" src=\"http://cdn.me.com/foo.js\"></script>");
        }

        [Fact]
        public void has_a_cdn_and_fallback()
        {
            theAsset.CdnUrl = "http://cdn.me.com/foo.js";
            theAsset.FallbackTest = "foo == null";

            var tag = new ScriptTag(null, x => x, theAsset);

            tag.ToString().ShouldContain("<script>if (foo == null)");
        }
    }
}