using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Content;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public class BasicScriptTagWriter : IScriptTagWriter
    {
        private readonly IContentRegistry _registry;
        private const string FALLBACK = "window.{0} || document.write('<script src=\"{1}\"><\\/script>')";

        public BasicScriptTagWriter(IContentRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<IScript> scripts)
        {
            return scripts.Select(x =>
            {
                // TODO -- is it possible that we could have something besides JavaScript?
                var scriptUrl = _registry.ScriptUrl(x.Name);
                var scriptTag = new HtmlTag("script").Attr("src", scriptUrl).Attr("type", "text/javascript");
                if(x.HasFallback)
                    scriptTag.After(new HtmlTag("script").Attr("type", "text/javascript")
                        .Text(FALLBACK.ToFormat(x.WindowVariableName, x.FallbackName)).Encoded(false));
                return scriptTag;
            });
        }


    }
}