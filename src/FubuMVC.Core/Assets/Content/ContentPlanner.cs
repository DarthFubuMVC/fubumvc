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
        private readonly IAssetPipeline _pipeline;
        private readonly ITransformerPolicyLibrary _library;

        public ContentPlanner(IAssetCombinationCache combinations, IAssetPipeline pipeline, ITransformerPolicyLibrary library)
        {
            _combinations = combinations;
            _pipeline = pipeline;
            _library = library;
        }

        public ContentPlan BuildPlanFor(string name)
        {
            var files = FindFiles(name);
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

        private void applyBatchedNonGlobalTransforms(ContentPlan plan, TransformerRequirements requirements)
        {
            // blow up if there is more than one batched transform
            var policies = requirements.AllBatchedTransformerPolicies();

            if (policies.Count() > 1)
            {
                throw new TooManyBatchedTransformationsException(policies.Select(x => x.TransformerType));
            }

            var policy = policies.SingleOrDefault();
            if (policy == null) return;

            batchGroups(policy, plan, requirements).Each(group =>
            {
                if (group.Count() == 1)
                {
                    plan.ApplyTransform(group.Single(), policy.TransformerType);
                }
                else
                {
                    var combo = plan.Combine(group);
                    plan.ApplyTransform(combo, policy.TransformerType);
                }
            });

        }

        private static IEnumerable<IList<IContentSource>> batchGroups(ITransformerPolicy policy, ContentPlan plan, TransformerRequirements requirements)
        {
            var sources = new Lazy<List<IContentSource>>();

            foreach (var source in plan.GetAllSources().ToList())
            {
                if (requirements.IsNextPolicy(source, policy))
                {
                    sources.Value.Add(source);
                    requirements.DequeueTransformer(source, policy);
                }
                else
                {
                    if (sources.IsValueCreated)
                    {
                        yield return sources.Value;
                        sources = new Lazy<List<IContentSource>>();
                    }
                }
            }

            if (sources.IsValueCreated)
            {
                yield return sources.Value;
            }
        }

        private static void combineWhateverIsLeft(ContentPlan plan)
        {
            plan.CombineAll();
        }

        private static void applyNonBatchedNonGlobalTransforms(IEnumerable<AssetFile> files, ContentPlan plan, TransformerRequirements requirements)
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

            var assetFile = _pipeline.Find(name);

            if (assetFile == null)
            {
                throw new ArgumentOutOfRangeException("No combination or asset file exists with the name " + name);
            }


            return new AssetFile[]{assetFile};
        }
    }



}