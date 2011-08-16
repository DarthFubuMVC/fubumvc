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
            page.Get<AssetRequirements>().Require(scriptName);
        }

        public static void OptionalScript(this IFubuPage page, string scriptName)
        {
            page.Get<AssetRequirements>().UseFileIfExists(scriptName);
        }

        // Tested manually
        public static TagList CSS(this IFubuPage page, params string[] cssNames)
        {
            return new TagList(page.Get<ICssLinkTagWriter>().Write(cssNames));
        }

        public static TagList WriteScriptTags(this IFubuPage page)
        {
            return page.Get<ScriptIncludeWriter>().ScriptTags();
        }

        public class ScriptIncludeWriter
        {
            private readonly AssetRequirements _requirements;
            private readonly IAssetTagWriter _writer;

            public ScriptIncludeWriter(AssetRequirements requirements, IAssetTagWriter writer)
            {
                _requirements = requirements;
                _writer = writer;
            }

            public TagList ScriptTags()
            {
                var scripts = _requirements.GetAssetsToRender();
                var tags = _writer.Write(scripts);

                return new TagList(tags);
            }
        }
    }
}