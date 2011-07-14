using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Content;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public interface ICssLinkTagWriter
    {
        IEnumerable<HtmlTag> Write(IEnumerable<string> stylesheets);
        IEnumerable<HtmlTag> WriteIfExists(IEnumerable<string> stylesheets);
    }

    public class CssLinkTagWriter : ICssLinkTagWriter
    {
        private readonly IContentRegistry _contentRegistry;

        public CssLinkTagWriter(IContentRegistry contentRegistry)
        {
            _contentRegistry = contentRegistry;
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<string> stylesheets)
        {
            return createCssTags(stylesheets, false);
        }

        public IEnumerable<HtmlTag> WriteIfExists(IEnumerable<string> stylesheets)
        {
            return createCssTags(stylesheets, true);
        }

        private IEnumerable<HtmlTag> createCssTags(IEnumerable<string> stylesheets, bool optional)
        {
            return stylesheets.Select(x => _contentRegistry.CssUrl(x, optional))
                .Where(x => x != null)
                .Select(url => new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", "text/css"));
        }
    }

}