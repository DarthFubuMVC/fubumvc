using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Runtime;
using HtmlTags;
using FubuMVC.Core.UI;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagWriter
    {
        TagList WriteAllTags();
        TagList WriteTags(MimeType mimeType, params string[] tags);
    }

    // Mostly depending on StoryTeller tests for this class
    public class AssetTagWriter : IAssetTagWriter
    {
        private readonly IAssetTagBuilder _builder;
        private readonly IAssetTagPlanCache _planCache;
        private readonly IAssetRequirements _requirements;

        public AssetTagWriter(IAssetTagPlanCache planCache, IAssetRequirements requirements, IAssetTagBuilder builder)
        {
            _planCache = planCache;
            _requirements = requirements;
            _builder = builder;
        }

        public TagList WriteAllTags()
        {
            var requests = _requirements.DequeueAssetsToRender();

            return requests.SelectMany(TagsForPlan).ToTagList();
        }

        private TagList writeTags(MimeType mimeType)
        {
            var plan = _requirements.DequeueAssetsToRender(mimeType);
            return TagsForPlan(plan).ToTagList();
        }

        public TagList WriteTags(MimeType mimeType, params string[] tags)
        {
            if (tags.Length == 0) return writeTags(mimeType);

            var plan = _requirements.DequeueAssets(mimeType,tags);
            return TagsForPlan(plan).ToTagList();
        }

        public IEnumerable<HtmlTag> TagsForPlan(AssetPlanKey key)
        {
            var plan = _planCache.PlanFor(key);
            return _builder.Build(plan);
        }
    }

    
}