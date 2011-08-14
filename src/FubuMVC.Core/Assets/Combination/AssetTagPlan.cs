using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using System.Linq;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetTagPlan
    {
        private readonly string _mimeType;
        private readonly IList<IAssetTagSubject> _subjects = new List<IAssetTagSubject>();

        public AssetTagPlan(string mimeType, IEnumerable<AssetFile> files)
        {
            _mimeType = mimeType;

            _subjects.AddRange(files);

        }

        public string MimeType
        {
            get { return _mimeType; }
        }

        public IList<IAssetTagSubject> Subjects
        {
            get { return _subjects; }
        }

        public bool TryCombination(AssetFileCombination combination)
        {
            var combinationCount = combination.Files.Count();
            var subjectCount = _subjects.Count;

            if (combinationCount > subjectCount) return false;

            var index = _subjects.IndexOf(combination.Files.First());
            if (index < 0) return false;

            var endIndex = index + combinationCount - 1;
            if (endIndex > subjectCount - 1) return false;

            var queue = new Queue<AssetFile>(combination.Files);
            for (int i = index; i < endIndex; i++)
            {
                var file = queue.Dequeue();
                if (_subjects[i] != file) return false;
            }

            combination.Files.Each(f => _subjects.Remove(f));

            _subjects.Insert(index, combination);

            return true;
        }
    }
}