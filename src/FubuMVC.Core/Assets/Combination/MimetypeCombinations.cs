using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    // TODO -- Extend this later when we hit configuration?
    // Nah, let something else do the work of reading the DSL and finding the asset files
    // TODO -- think about combining this into AssetCombinationCache 
    public class CombinationCandidateCache
    {
        private readonly Cache<MimeType, MimetypeCombinations> _combinations = new Cache<MimeType, MimetypeCombinations>(mimeType => new MimetypeCombinations(mimeType));

        public void AddFiles(MimeType mimeType, string name, IEnumerable<AssetFile> files)
        {
            _combinations[mimeType][name].AddFiles(files);
        }

        public IEnumerable<CombinationCandidate> OrderedCombinationCandidatesFor(MimeType mimeType)
        {
            return _combinations[mimeType].GetAll().OrderByDescending(x => x.Length);
        }
    }

    public class MimetypeCombinations : Cache<string, CombinationCandidate>
    {
        public MimetypeCombinations(MimeType mimeType) : base(name => new CombinationCandidate(mimeType, name))
        {
        }
    }

    public interface IAssetCombinationCache
    {
        void StoreCombination(string mimeType, AssetFileCombination combination);
        IEnumerable<AssetFileCombination> OrderedListOfCombinations(MimeType mimeType);
    }

    public class AssetCombinationCache : IAssetCombinationCache
    {
        private readonly Cache<string, AssetCombinationSet> _combinations
            = new Cache<string, AssetCombinationSet>(mimeType => new AssetCombinationSet(mimeType));

        public void StoreCombination(string mimeType, AssetFileCombination combination)
        {
            _combinations[mimeType][combination.Name] = combination;
        }

        public IEnumerable<AssetFileCombination> OrderedListOfCombinations(MimeType mimeType)
        {
            throw new NotImplementedException();
        }
    }
}