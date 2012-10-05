using System.Collections.Generic;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagPlanCache
    {
        AssetTagPlan PlanFor(MimeType mimeType, IEnumerable<string> names);
        AssetTagPlan PlanFor(AssetPlanKey key);
    }
}