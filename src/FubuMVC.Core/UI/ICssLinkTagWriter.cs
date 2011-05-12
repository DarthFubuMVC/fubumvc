using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Content;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public interface ICssLinkTagWriter
    {
        IEnumerable<HtmlTag> Write(IEnumerable<string> stylesheets);
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
            return stylesheets.Select(stylesheet =>
            {
                var url = _contentRegistry.CssUrl(stylesheet);
                return new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", "text/css");
            });
        }
    }

}