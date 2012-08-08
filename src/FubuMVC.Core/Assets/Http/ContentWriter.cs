using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FubuCore;
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
        private readonly IAssetPipeline _pipeline;
        private readonly IContentPlanCache _cache;
        private readonly IContentPipeline _contentPipeline;
        private readonly IOutputWriter _writer;

        public ContentWriter(IAssetPipeline pipeline, IContentPlanCache cache, IContentPipeline contentPipeline,
                             IOutputWriter writer)
        {
            _pipeline = pipeline;
            _cache = cache;
            _contentPipeline = contentPipeline;
            _writer = writer;
        }

        public IEnumerable<AssetFile> Write(AssetPath asset)
        {
            if(!FubuMode.InDevelopment())
            {
                var file = _pipeline.Find(asset);
                var lastMod = File.GetLastWriteTimeUtc(file.FullPath);
                _writer.AppendHeader(HttpResponseHeader.LastModified, lastMod.ToString("R"));
                //setting max-age to 8 hours
                _writer.AppendHeader(HttpResponseHeader.CacheControl, "{0}={1}".ToFormat("max-age",8 * 60 * 60));
                //setting expires to one year from last modified. That's what facebook does
                _writer.AppendHeader(HttpResponseHeader.Expires, lastMod.AddYears(1).ToString("R"));
            }

            if (asset.IsImage())
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
            var contents = source.GetContent(_contentPipeline);

            _writer.Write(source.Files.First().MimeType, contents);

            return source.Files;
        }

        private IEnumerable<AssetFile> writeBinary(AssetPath asset)
        {
            var file = _pipeline.Find(asset);
            _writer.WriteFile(file.MimeType, file.FullPath, null);

            return new AssetFile[]{file};
        }
    }
}