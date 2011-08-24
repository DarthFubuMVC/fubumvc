using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public class ReadFileSource : IContentSource
    {
        private readonly AssetFile _file;

        public ReadFileSource(AssetFile file)
        {
            _file = file;
        }

        public string GetContent(ITransformContext context)
        {
            return context.ReadContentsFrom(_file.FullPath);
        }

        public IEnumerable<AssetFile> Files
        {
            get { yield return _file; }
        }
    }
}