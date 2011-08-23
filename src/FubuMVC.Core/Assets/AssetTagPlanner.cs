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
        private readonly IAssetPipeline _pipeline;
        private readonly ICombinationDeterminationService _combinations;

        public AssetTagPlanner(IAssetPipeline pipeline, ICombinationDeterminationService combinations)
        {
            _pipeline = pipeline;
            _combinations = combinations;
        }

        public AssetTagPlan BuildPlan(MimeType mimeType, IEnumerable<string> names)
        {
            var plan = new AssetTagPlan(mimeType);
            plan.AddSubjects(FindSubjects(names));
            
            if (plan.Subjects.Any(x => x.MimeType != mimeType))
            {
                var message = "Files {0} have mixed mimetype's".ToFormat(names.Join(", "));
                throw new MixedMimetypeException(message);
            }

            _combinations.TryToReplaceWithCombinations(plan);

            return plan;
        }

        public IEnumerable<IAssetTagSubject> FindSubjects(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var file = _pipeline.Find(name);
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