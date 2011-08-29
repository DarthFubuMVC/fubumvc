using System;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuCore;

namespace FubuMVC.Core.Assets.Content
{
    public class ContentPlanCache : IContentPlanCache
    {
        private readonly Cache<string, ContentPlan> _plans;

        public ContentPlanCache(IContentPlanner planner)
        {
            // TODO -- strongly consider going to AssetPath here.
            _plans = new Cache<string, ContentPlan>(planner.BuildPlanFor);
        }

        public ContentPlan PlanFor(string name)
        {
            return _plans[name];
        }

        public IContentSource SourceFor(AssetPath path)
        {
            if (path.Package.IsNotEmpty()) return _plans[path.ToFullName()];

            return PlanFor(path.Name);
        }
    }
}