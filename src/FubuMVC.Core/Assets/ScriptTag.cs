using System;
using FubuCore;
using FubuMVC.Core.Http.Scenarios;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class ScriptTag : HtmlTag
    {
        public ScriptTag(Func<string, string> toFullUrl, Asset asset, string defaultUrl = null)
            : base("script")
        {
            // http://stackoverflow.com/a/1288319/75194 
            Attr("type", "text/javascript");

            if (asset == null)
            {
                Attr("src", toFullUrl(defaultUrl));
                return;
            }

            if (asset.CdnUrl.IsNotEmpty())
            {
                Attr("src", asset.CdnUrl);
                if (asset.FallbackTest.IsNotEmpty() && asset.File != null)
                {
                    Next = new HtmlTag("script");
                    var text = "if ({0}) document.write(unescape(\"%3Cscript src='{1}' type='text/javascript'%3E%3C/script%3E\"));".ToFormat(asset.FallbackTest, asset.Url);

                    Next.Encoded(false);
                    Next.Text(text);
                }

                return;
            }

            var url = asset.Url;
            if (FubuMode.InDevelopment() && asset.File != null)
            {
                url += "?Etag=" + asset.File.Etag();
            }

            Attr("src", toFullUrl(url));
        }
    }
}