using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Content;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagWriter
    {
    }

    public class AssetTagWriter : IAssetTagWriter
    {
        private readonly IAssetTagPlanCache _planCache;
        private readonly IAssetRequirements _requirements;
        private readonly IAssetTagBuilder _builder;

        public AssetTagWriter(IAssetTagPlanCache planCache, IAssetRequirements requirements, IAssetTagBuilder builder)
        {
            _planCache = planCache;
            _requirements = requirements;
            _builder = builder;
        }

        public IEnumerable<HtmlTag> WriteAllTags()
        {
            var requests = _requirements.DequeueAssetsToRender();

            return requests.SelectMany(TagsForPlan);
        }

        public IEnumerable<HtmlTag> WriteTags(MimeType mimeType)
        {
            var plan = _requirements.DequeueAssetsToRender(mimeType);
            return TagsForPlan(plan);
        }

        public IEnumerable<HtmlTag> TagsForPlan(AssetPlanKey key)
        {
            var plan = _planCache.PlanFor(key);
            return _builder.Build(plan);
        }
    }
}