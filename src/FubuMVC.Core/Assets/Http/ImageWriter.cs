using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class ImageWriter : IImageWriter
    {
        private readonly IOutputWriter _writer;
        private readonly IAssetPipeline _pipeline;
        private readonly IResponseCaching _caching;

        public ImageWriter(IOutputWriter writer, IAssetPipeline pipeline, IResponseCaching caching)
        {
            _writer = writer;
            _pipeline = pipeline;
            _caching = caching;
        }

        public void WriteImageToOutput(string name)
        {
            // TODO -- error handling?  It's sort of coverted by the AssetTagPlan as is

            var file = _pipeline.Find(name);
            _writer.WriteFile(file.MimeType.Value, file.FullPath, null);
            _caching.CacheRequestAgainstFileChanges(file.FullPath);
        }
    }
}