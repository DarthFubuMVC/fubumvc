using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Combination
{
    public class ScriptFileCombination : AssetFileCombination
    {
        public ScriptFileCombination(IEnumerable<AssetFile> files) : base(null, ".js", files)
        {
        }

        public override AssetFolder? Folder
        {
            get { return AssetFolder.scripts; }
        }

        public override string MimeType
        {
            get { return MimeTypeProvider.JAVASCRIPT; }
        }
    }
}