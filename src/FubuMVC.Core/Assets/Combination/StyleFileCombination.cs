using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class StyleFileCombination : AssetFileCombination
    {
        public StyleFileCombination(string folder, IEnumerable<AssetFile> files) : base(folder, MimeType.Css, files)
        {
        }

        public StyleFileCombination(string name, string folder, IEnumerable<AssetFile> files) : this(folder, files)
        {
            Name = name;
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.styles; }
        }
    }
}