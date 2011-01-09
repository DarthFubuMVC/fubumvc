using System;
using FubuMVC.Core.UI.Scripts;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    // Tested by StoryTeller
    public static class ScriptAndCssPageExtensions
    {
        public static void Script(this IFubuPage page, string scriptName)
        {
            page.Get<ScriptRequirements>().Require(scriptName);
        }

        public static void OptionalScript(this IFubuPage page, string scriptName)
        {
            page.Get<ScriptRequirements>().UseFileIfExists(scriptName);
        }

        // Tested manually
        public static HtmlTag CSS(this IFubuPage page, string cssName)
        {
            throw new NotImplementedException();
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
                var scripts = _scripts.GetScripts(_requirements.AllScriptNames());
                var tags = _writer.Write(scripts);

                return new TagList(tags);
            }
        }
    }
}