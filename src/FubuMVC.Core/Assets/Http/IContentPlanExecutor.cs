using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Http
{
    public interface IContentPlanExecutor
    {
        void Execute(AssetPath path, ProcessContentAction continuation);
    }
}