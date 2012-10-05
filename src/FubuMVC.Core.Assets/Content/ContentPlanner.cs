using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public class ContentPlanner : IContentPlanner
    {
        private readonly IAssetCombinationCache _combinations;
        private readonly ITransformerPolicyLibrary _library;
        private readonly IAssetFileGraph _fileGraph;

        public ContentPlanner(IAssetCombinationCache combinations, IAssetFileGraph fileGraph,
                              ITransformerPolicyLibrary library)
        {
            _combinations = combinations;
            _fileGraph = fileGraph;
            _library = library;
        }

        public ContentPlan BuildPlanFor(string name)
        {
            var files = FindFiles(name);

            if (!files.Any())
            {
                return new ContentPlan(name, files);
            }

            var requirements = new TransformerRequirements(_library);

            var plan = new ContentPlan(name, files);

            applyNonBatchedNonGlobalTransforms(files, plan, requirements);

            applyBatchedNonGlobalTransforms(plan, requirements);

            applyGlobalTransforms(plan);

            combineWhateverIsLeft(plan);

            return plan;
        }

        private void applyGlobalTransforms(ContentPlan plan)
        {
            var globalPolicies = _library.FindGlobalPoliciesFor(plan.MimeType);
            globalPolicies.Each(policy =>
            {
                if (policy.MustBeBatched())
                {
                    plan.CombineAll();
                }

                plan.GetAllSources().Each(s => plan.ApplyTransform(s, policy.TransformerType));
            });
        }

        private static void applyBatchedNonGlobalTransforms(ContentPlan plan, TransformerRequirements requirements)
        {
            var policy = findBatchedTransformerPolicy(requirements);
            if (policy == null) return;


            var groups = new AssetGrouper<IContentSource>()
                .GroupSubjects(plan.GetAllSources(), source => requirements.IsNextPolicy(source, policy));

            groups.Each(group =>
            {
                if (group.Count == 1)
                {
                    plan.ApplyTransform(group.Single(), policy.TransformerType);
                }
                else
                {
                    var combo = plan.Combine(group);
                    plan.ApplyTransform(combo, policy.TransformerType);
                }

                group.Each(s => requirements.DequeueTransformer(s, policy));
            });
        }

        private static ITransformerPolicy findBatchedTransformerPolicy(TransformerRequirements requirements)
        {
            var policies = requirements.AllBatchedTransformerPolicies();

            // There can only be one! -- gotta say it in the Highlander voice
            if (policies.Count() > 1)
            {
                throw new TooManyBatchedTransformationsException(policies.Select(x => x.TransformerType));
            }

            return policies.SingleOrDefault();
        }


        private static void combineWhateverIsLeft(ContentPlan plan)
        {
            plan.CombineAll();
        }

        private static void applyNonBatchedNonGlobalTransforms(IEnumerable<AssetFile> files, ContentPlan plan,
                                                               TransformerRequirements requirements)
        {
            foreach (var file in files)
            {
                var source = plan.FindForFile(file);
                var policies = requirements.PoliciesFor(file);

                while (policies.Any() && !policies.Peek().MustBeBatched())
                {
                    var policy = policies.Dequeue();
                    source = plan.ApplyTransform(source, policy.TransformerType);
                }
            }
        }


        public IEnumerable<AssetFile> FindFiles(string name)
        {
            var combination = _combinations.FindCombination(name);
            if (combination != null)
            {
                return combination.Files;
            }

            var assetFile = _fileGraph.Find(name);

            if (assetFile == null)
            {
                return Enumerable.Empty<AssetFile>();
            }


            return new[]{assetFile};
        }
    }
}