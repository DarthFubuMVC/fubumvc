using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Assets.Combination
{
    public class StyleFileCombination : AssetFileCombination
    {
        public StyleFileCombination(string folder, IEnumerable<AssetFile> files) : base(folder, MimeType.Css, files)
        {
        }

        public StyleFileCombination(string name, string folder, IEnumerable<AssetFile> files) : this(folder, files)
        {
            Name = folder.IsNotEmpty() ? folder + "/" + name : name;

        }

        public override AssetFolder Folder
        {
            get { return AssetFolder.styles; }
        }
    }
}