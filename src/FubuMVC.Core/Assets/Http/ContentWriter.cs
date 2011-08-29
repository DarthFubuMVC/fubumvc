using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class ContentWriter : IContentWriter
    {
        private readonly IImageWriter _images;
        private readonly IContentPlanExecutor _executor;
        private readonly IResponseCaching _caching;
        private readonly IOutputWriter _writer;

        public ContentWriter(IImageWriter images, IContentPlanExecutor executor, IResponseCaching caching, IOutputWriter writer)
        {
            _images = images;
            _executor = executor;
            _caching = caching;
            _writer = writer;
        }

        public void WriteContent(IEnumerable<string> routeParts)
        {
            var path = new AssetPath(routeParts);
            if (path.IsImage())
            {
                _images.WriteImageToOutput(path.ToFullName());
            }
            else
            {
                // TODO -- have to deal with the [package]:scripts/
                // think it'll just be testing
                _executor.Execute(path, (contents, files) =>
                {
                    _caching.CacheRequestAgainstFileChanges(files.Select(x => x.FullPath));
                    _writer.Write(files.First().MimeType, contents);
                });
            }

            
        }

        
    }
}