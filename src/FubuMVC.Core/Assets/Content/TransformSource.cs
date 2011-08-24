using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public class TransformSource<T> : IContentSource where T : IAssetTransformer
    {
        private readonly IContentSource _inner;

        public TransformSource(IContentSource inner)
        {
            _inner = inner;
        }

        public string GetContent(IContentPipeline pipeline)
        {
            var transformer = pipeline.GetTransformer<T>();
            var innerContents = _inner.GetContent(pipeline);

            return transformer.Transform(innerContents, Files);
        }

        public IEnumerable<AssetFile> Files
        {
            get { return _inner.Files; }
        }
    }
}