using FubuCore.Util;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetCombinationSet : Cache<string, AssetFileCombination>
    {
        private readonly MimeType _mimeType;

        public AssetCombinationSet(MimeType mimeType)
        {
            _mimeType = mimeType;
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }
    }
}