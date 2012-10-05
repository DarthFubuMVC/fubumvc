using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public interface IContentWriter
    {
        IEnumerable<AssetFile> Write(AssetPath asset);
    }

    public class ContentWriter : IContentWriter
    {
        private readonly IAssetFileGraph _fileGraph;
        private readonly IContentPlanCache _cache;
        private readonly IContentPipeline _contentPipeline;
        private readonly IOutputWriter _writer;

        public ContentWriter(IAssetFileGraph fileGraph, IContentPlanCache cache, IContentPipeline contentPipeline,
                             IOutputWriter writer)
        {
            _fileGraph = fileGraph;
            _cache = cache;
            _contentPipeline = contentPipeline;
            _writer = writer;
        }

        public IEnumerable<AssetFile> Write(AssetPath asset)
        {
            if (asset.IsBinary())
            {
                return writeBinary(asset);
            }
            
            // TODO -- have to deal with the [package]:scripts/
            // think it'll just be testing
            return writeTextualAsset(asset);
        }

        private IEnumerable<AssetFile> writeTextualAsset(AssetPath asset)
        {
            var source = _cache.SourceFor(asset);
            if (source.Files.Any())
            {
                var contents = source.GetContent(_contentPipeline);
                _writer.Write(source.Files.First().MimeType, contents);
            }


            return source.Files;
        }

        private IEnumerable<AssetFile> writeBinary(AssetPath asset)
        {
            var file = _fileGraph.Find(asset);

            if (file == null)
            {
                return Enumerable.Empty<AssetFile>();
            }

            _writer.WriteFile(file.MimeType, file.FullPath, null);

            return new AssetFile[]{file};
        }
    }
}