using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    // Given an enumerable of AssetFile's, make the transformation plan
    public class ContentPlanner
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
            throw new NotImplementedException();

            var files = FindFiles(name);
            var requirements = new TransformerRequirements(_library);

            // Step 1, for each file, grab anything 
            // for each file, grab the source, and pull in each policy that is not batched.  Dequeue all the policies

            // Step 2, for the batching stuff, combine, then batch and pop the policy
            // if you can't pull of the batch, blow chunks.

            // Step 3, do the globals, watch the batching

            // Step 4, combine everything

            var plan = new ContentPlan(name, files);
            

        }

        public IEnumerable<AssetFile> FindFiles(string name)
        {
            throw new NotImplementedException();
            var combination = _combinations.FindCombination(name);
            if (combination != null)
            {
                return combination.Files;
            }

            var assetFile = _pipeline.Find(name);
            return new AssetFile[]{assetFile};
        }
    }

    public class TransformerRequirements
    {
        private readonly ITransformerPolicyLibrary _library;
        private readonly Cache<AssetFile, Queue<ITransformerPolicy>> _policies;

        public TransformerRequirements(ITransformerPolicyLibrary library)
        {
            _library = library;
            _policies = new Cache<AssetFile, Queue<ITransformerPolicy>>(findPolicies);
        }

        public Queue<ITransformerPolicy> PoliciesFor(AssetFile file)
        {
            return _policies[file];
        }

        private Queue<ITransformerPolicy> findPolicies(AssetFile file)
        {
            throw new NotImplementedException();
        }
    }

    public class ContentPlanCache
    {
        // keeps transformation plan per name
        // keeps transformer 
    }
}