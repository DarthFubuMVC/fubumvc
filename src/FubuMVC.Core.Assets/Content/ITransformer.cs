using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public interface ITransformer
    {
        string Transform(string contents, IEnumerable<AssetFile> files);
    }
}