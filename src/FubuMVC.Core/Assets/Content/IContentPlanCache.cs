namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPlanCache
    {
        ContentPlan PlanFor(string name);
        IContentSource SourceFor(string name);
    }
}