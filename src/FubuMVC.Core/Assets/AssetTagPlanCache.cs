using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class AssetTagPlanCache : IAssetTagPlanCache
    {
        private readonly IAssetTagPlanner _planner;
        private readonly Cache<AssetPlanKey, AssetTagPlan> _plans;


        public AssetTagPlanCache(IAssetTagPlanner planner)
        {
            _planner = planner;

            _plans = new Cache<AssetPlanKey, AssetTagPlan>(key => _planner.BuildPlan(key));
        }

        public AssetTagPlan PlanFor(MimeType mimeType, IEnumerable<string> names)
        {
            var key = new AssetPlanKey(mimeType, names);
            return PlanFor(key);
        }

        public AssetTagPlan PlanFor(AssetPlanKey key)
        {
            return _plans[key];
        }
    }
}