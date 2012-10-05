using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPlanCache
    {
        ContentPlan PlanFor(string name);
        IContentSource SourceFor(AssetPath path);
    }
}