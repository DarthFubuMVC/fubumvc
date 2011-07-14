using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Content;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public class BasicScriptTagWriter : IScriptTagWriter
    {
        private readonly IContentRegistry _registry;

        public BasicScriptTagWriter(IContentRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<string> scripts)
        {
            return scripts.Select(x =>
            {
                // TODO -- is it possible that we could have something besides JavaScript?
                var scriptUrl = _registry.ScriptUrl(x, false);
                return new HtmlTag("script").Attr("src", scriptUrl).Attr("type", "text/javascript");
            });
        }


    }
}