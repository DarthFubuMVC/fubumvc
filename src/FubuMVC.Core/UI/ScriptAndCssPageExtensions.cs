using System;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    // Tested by StoryTeller
    public static class ScriptAndCssPageExtensions
    {
        public static void Script(this IFubuPage page, string scriptName)
        {
            page.Get<IAssetRequirements>().Require(scriptName);
        }

        public static void OptionalScript(this IFubuPage page, string scriptName)
        {
            page.Get<IAssetRequirements>().UseFileIfExists(scriptName);
        }

        // Tested manually
        public static TagList CSS(this IFubuPage page, params string[] cssNames)
        {
            return new TagList(page.Get<ICssLinkTagWriter>().Write(cssNames));
        }

        public static TagList WriteScriptTags(this IFubuPage page)
        {
            throw new NotImplementedException();
            //return page.Get<ScriptIncludeWriter>().ScriptTags();
        }


    }
}