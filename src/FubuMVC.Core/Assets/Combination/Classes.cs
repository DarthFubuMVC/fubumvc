using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetCombinationSet : Cache<string, AssetFileCombination>
    {
        private readonly string _mimeType;

        public AssetCombinationSet(string mimeType)
        {
            _mimeType = mimeType;
        }

        public string MimeType
        {
            get { return _mimeType; }
        }
    }



    public interface ICombinationPolicy
    {
        MimeType MimeType { get; }
        IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan);
    }

    // TODO -- combination should allow you to override the name!
    public class CombinationCandidate : ICombinationPolicy
    {
        private readonly MimeType _mimeType;
        private readonly string _name;
        private readonly IList<AssetFile> _files = new List<AssetFile>();

        public CombinationCandidate(MimeType mimeType, string name)
        {
            _mimeType = mimeType;
            _name = name;
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
            throw new NotImplementedException();
        }

        public int Length
        {
            get
            {
                return _files.Count();
            }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}