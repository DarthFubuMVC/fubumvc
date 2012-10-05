using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Assets
{
    public class AssetTagPlanner : IAssetTagPlanner
    {
        private readonly IAssetFileGraph _fileGraph;
        private readonly ICombinationDeterminationService _combinations;

        public AssetTagPlanner(IAssetFileGraph fileGraph, ICombinationDeterminationService combinations)
        {
            _fileGraph = fileGraph;
            _combinations = combinations;
        }

        public AssetTagPlan BuildPlan(MimeType mimeType, IEnumerable<string> names)
        {
            var plan = new AssetTagPlan(mimeType);
            plan.AddSubjects(FindSubjects(names));

            validateMatchingMimetypes(mimeType, plan, names);

            if (plan.Subjects.Count == 1)
            {
                return plan;
            }

            _combinations.TryToReplaceWithCombinations(plan);

            return plan;
        }

        private static void validateMatchingMimetypes(MimeType mimeType, AssetTagPlan plan, IEnumerable<string> names)
        {
            if (plan.Subjects.Any(x => x.MimeType != mimeType))
            {
                var message = "Files {0} have mixed mimetype's".ToFormat(names.Join(", "));
                throw new MixedMimetypeException(message);
            }
        }

        public AssetTagPlan BuildPlan(AssetPlanKey key)
        {
            return BuildPlan(key.MimeType, key.Names);
        }

        public IEnumerable<IAssetTagSubject> FindSubjects(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var file = _fileGraph.Find(name);
                if (file != null)
                {
                    yield return file;
                }
                else
                {
                    yield return new MissingAssetTagSubject(name);
                }
            }
        }


    }
}