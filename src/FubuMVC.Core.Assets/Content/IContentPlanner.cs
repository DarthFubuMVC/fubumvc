namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPlanner
    {
        ContentPlan BuildPlanFor(string name);
    }
}