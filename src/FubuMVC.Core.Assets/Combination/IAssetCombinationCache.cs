using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public interface IAssetCombinationCache
    {
        void StoreCombination(MimeType mimeType, AssetFileCombination combination);
        IEnumerable<AssetFileCombination> OrderedListOfCombinations(MimeType mimeType);

        void AddFilesToCandidate(MimeType mimeType, string name, IEnumerable<AssetFile> files);
        IEnumerable<CombinationCandidate> OrderedCombinationCandidatesFor(MimeType mimeType);

        AssetFileCombination FindCombination(string name);
    }
}