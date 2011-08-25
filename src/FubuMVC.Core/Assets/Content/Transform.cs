using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public class Transform<T> : IContentSource where T : ITransformer
    {
        private readonly IContentSource _inner;

        public Transform(IContentSource inner)
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

        public IEnumerable<IContentSource> InnerSources
        {
            get { yield return _inner; }
        }

        public override string ToString()
        {
            return "Transform:" + typeof (T).Name;
        }
    }
}