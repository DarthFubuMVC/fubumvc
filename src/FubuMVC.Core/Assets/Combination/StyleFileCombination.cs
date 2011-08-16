using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class StyleFileCombination : AssetFileCombination
    {
        public StyleFileCombination(string folder, IEnumerable<AssetFile> files) : base(folder, ".css", files)
        {
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.styles; }
        }

        public override MimeType MimeType
        {
            get { return MimeType.Css; }
        }
    }
}