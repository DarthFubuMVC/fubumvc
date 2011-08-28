using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    // TODO -- simplify this down and eliminate the storage by MimeType
    public class AssetCombinationCache : IAssetCombinationCache
    {
        private readonly Cache<MimeType, AssetCombinationSet> _combinations
            = new Cache<MimeType, AssetCombinationSet>(mimeType => new AssetCombinationSet(mimeType));

        public void StoreCombination(MimeType mimeType, AssetFileCombination combination)
        {
            _combinations[mimeType][combination.Name] = combination;
        }

        public IEnumerable<AssetFileCombination> OrderedListOfCombinations(MimeType mimeType)
        {
            return _combinations[mimeType].GetAll().OrderByDescending(x => x.Length);
        }

        private readonly Cache<MimeType, MimetypeCombinations> _candidates = new Cache<MimeType, MimetypeCombinations>(mimeType => new MimetypeCombinations(mimeType));

        public void AddFilesToCandidate(MimeType mimeType, string name, IEnumerable<AssetFile> files)
        {
            _candidates[mimeType][name].AddFiles(files);
        }

        public IEnumerable<CombinationCandidate> OrderedCombinationCandidatesFor(MimeType mimeType)
        {
            return _candidates[mimeType].GetAll().OrderByDescending(x => x.Length);
        }

        public AssetFileCombination FindCombination(string name)
        {
            return _combinations.GetAll().SelectMany(x => x.GetAll()).FirstOrDefault(x => x.Name == name);
        }
    }
}