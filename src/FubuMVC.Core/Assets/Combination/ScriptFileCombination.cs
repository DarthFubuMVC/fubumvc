using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class ScriptFileCombination : AssetFileCombination
    {
        public ScriptFileCombination(IEnumerable<AssetFile> files) : base(null, MimeType.Javascript, files)
        {
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.scripts; }
        }
    }
}