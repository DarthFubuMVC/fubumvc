using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class CombinationCandidate : ICombinationPolicy
    {
        private readonly MimeType _mimeType;
        private readonly string _name;
        private readonly IList<AssetFile> _files = new List<AssetFile>();

        private readonly Func<IEnumerable<AssetFile>, AssetFileCombination> _combinationBuilder = files =>
        {
            throw new NotImplementedException();
        };

        public string Folder { get; set; }

        public CombinationCandidate(MimeType mimeType, string name)
        {
            _mimeType = mimeType;
            _name = name;

            if (_mimeType == Runtime.MimeType.Javascript)
            {
                _combinationBuilder = files => new ScriptFileCombination(_name, files);
            }

            if (_mimeType == Runtime.MimeType.Css)
            {
                _combinationBuilder = files => new StyleFileCombination(_name, Folder, files);
            }
        }

        public CombinationCandidate(MimeType mimeType, string name, IEnumerable<AssetFile> files) : this(mimeType, name)
        {
            _files.AddRange(files);
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }

        public void AddFiles(IEnumerable<AssetFile> files)
        {
            _files.AddRange(files);
        }

        public IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan)
        {
            if (plan.MimeType != MimeType) yield break;


            foreach (var assetFile in _files)
            {
                var sequence = plan.TryFindSequenceStartingWith(assetFile, _files.Count);
                if (sequence == null) continue;

                if (_files.All(sequence.Contains))
                {
                    yield return _combinationBuilder(sequence);
                    break;
                }
            }
        }

        public int Length
        {
            get
            {
                return _files.Count();
            }
        }

        public IList<AssetFile> Files
        {
            get { return _files; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}