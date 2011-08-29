namespace FubuMVC.Core.Assets.Http
{
    public interface IContentPlanExecutor
    {
        void Execute(string name, ProcessContentAction continuation);
    }
}