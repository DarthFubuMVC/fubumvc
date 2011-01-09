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

        public IEnumerable<HtmlTag> Write(IEnumerable<IScript> scripts)
        {
            return scripts.Select(x =>
            {
                // TODO -- is it possible that we could have something besides JavaScript?
                return new HtmlTag("script").Attr("src", _registry.ScriptUrl(x.Name)).Attr("type", "text/javascript");
            });
        }


    }
}