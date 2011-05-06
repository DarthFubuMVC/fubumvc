using System;
using FubuMVC.Core.Content;
using FubuMVC.Core.UI.Scripts;
using FubuMVC.Core.View;
using HtmlTags;
using System.Linq;

namespace FubuMVC.Core.UI
{
    // Tested by StoryTeller
    public static class ScriptAndCssPageExtensions
    {
        public static void Script(this IFubuPage page, string scriptName)
        {
            page.Get<ScriptRequirements>().ConfiguredScript(scriptName);
        }

        public static void OptionalScript(this IFubuPage page, string scriptName)
        {
            page.Get<ScriptRequirements>().UseFileIfExists(scriptName);
        }

        public static void PageScript(this IFubuPage page, string scriptName)
        {
            page.Get<ScriptRequirements>().PageScript(scriptName);
        }

        // Tested manually
        public static HtmlTag CSS(this IFubuPage page, string cssName)
        {
            // TODO:  Put a hook here for things like LESS for .Net
            var url = page.Get<IContentRegistry>().CssUrl(cssName);

            return new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", "text/css");
        }

        public static TagList WriteScriptTags(this IFubuPage page)
        {
            return page.Get<ScriptIncludeWriter>().ScriptTags();
        }

        public class ScriptIncludeWriter
        {
            private readonly ScriptGraph _scripts;
            private readonly ScriptRequirements _requirements;
            private readonly IScriptTagWriter _writer;

            public ScriptIncludeWriter(ScriptGraph scripts, ScriptRequirements requirements, IScriptTagWriter writer)
            {
                _scripts = scripts;
                _requirements = requirements;
                _writer = writer;
            }

            public TagList ScriptTags()
            {
                var configuredScripts = _scripts.GetScripts(_requirements.AllConfiguredScriptNames());
                var pageScriptNames = _requirements.AllPageScriptNames();

                var allScripts = configuredScripts.Select(x => x.Name).Concat(pageScriptNames);
                
                var tags = _writer.Write(allScripts);

                return new TagList(tags);
            }
        }
    }
}