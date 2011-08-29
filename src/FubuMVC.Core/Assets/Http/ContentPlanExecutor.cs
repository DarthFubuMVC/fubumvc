using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Http
{
    public class ContentPlanExecutor : IContentPlanExecutor
    {
        private readonly IContentPlanCache _cache;
        private readonly IContentPipeline _pipeline;

        public ContentPlanExecutor(IContentPlanCache cache, IContentPipeline pipeline)
        {
            _cache = cache;
            _pipeline = pipeline;
        }

        public void Execute(AssetPath path, ProcessContentAction continuation)
        {
            var source = _cache.SourceFor(path);
            var contents = source.GetContent(_pipeline);

            continuation(contents, source.Files);
        }
    }
}