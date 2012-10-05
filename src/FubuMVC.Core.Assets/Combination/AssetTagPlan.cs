using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class AssetTagPlan
    {
        private readonly MimeType _mimeType;
        private readonly IList<IAssetTagSubject> _subjects = new List<IAssetTagSubject>();

        public AssetTagPlan(MimeType mimeType, IEnumerable<AssetFile> files) : this(mimeType)
        {
            _subjects.AddRange(files);
        }

        public AssetTagPlan(MimeType mimeType)
        {
            _mimeType = mimeType;
        }

        public void AddSubjects(IEnumerable<IAssetTagSubject> subjects)
        {
            _subjects.AddRange(subjects);
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }

        public IList<IAssetTagSubject> Subjects
        {
            get { return _subjects; }
        }

        public IList<AssetFile> TryFindSequenceStartingWith(AssetFile assetFile, int combinationCount)
        {
            var index = _subjects.IndexOf(assetFile);
            if (index < 0) return null;

            var endIndex = index + combinationCount - 1;
            if (endIndex > _subjects.Count - 1) return null;

            var subjectSection = _subjects.Skip(index).Take(combinationCount);

            if (subjectSection.All(x => x is AssetFile))
            {
                return subjectSection.OfType<AssetFile>().ToList();
            }

            return null;
        }

        public virtual bool TryCombination(AssetFileCombination combination)
        {
            var combinationCount = combination.Files.Count();

            if (combinationCount > _subjects.Count) return false;

            var assetFile = combination.Files.First();

            var index = _subjects.IndexOf(assetFile);
            if (index < 0) return false;

            var list = TryFindSequenceStartingWith(assetFile, combinationCount);
            if (list == null) return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != combination.FileAt(i)) return false;
            }

            replaceFilesWithCombination(combination, index);

            return true;
        }

        private void replaceFilesWithCombination(AssetFileCombination combination, int index)
        {
            combination.Files.Each(f => _subjects.Remove(f));

            _subjects.Insert(index, combination);
        }

        public IEnumerable<MissingAssetTagSubject> RemoveMissingAssets()
        {
            var missing = _subjects.OfType<MissingAssetTagSubject>().ToList();
            _subjects.RemoveAll(x => x is MissingAssetTagSubject);

            return missing;
        }

        public static AssetTagPlan For(MimeType mimeType, params IAssetTagSubject[] subjects)
        {
            var plan = new AssetTagPlan(mimeType);
            plan.AddSubjects(subjects);

            return plan;
        }
    }
}