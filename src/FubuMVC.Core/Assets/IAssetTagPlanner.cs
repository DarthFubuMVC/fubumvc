using System.Collections.Generic;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagPlanner
    {
        AssetTagPlan BuildPlan(MimeType mimeType, IEnumerable<string> names);
        AssetTagPlan BuildPlan(AssetPlanKey key);
    }
}