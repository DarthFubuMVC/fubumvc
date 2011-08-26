using System;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Content
{
    public class ContentPlanCache : IContentPlanCache
    {
        private readonly Cache<string, ContentPlan> _plans;

        public ContentPlanCache(IContentPlanner planner)
        {
            _plans = new Cache<string, ContentPlan>(planner.BuildPlanFor);
        }

        public ContentPlan PlanFor(string name)
        {
            return _plans[name];
        }
    }
}