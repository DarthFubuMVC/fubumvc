using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using System.Linq;

namespace FubuMVC.Core.Assets.Content
{
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
            return new Queue<ITransformerPolicy>(_library.FindPoliciesFor(file));
        }

        public IEnumerable<ITransformerPolicy> AllBatchedTransformerPolicies()
        {
            return _policies
                .GetAll()
                .SelectMany(x => x)
                .Where(x => x.ActionType == ActionType.BatchedTransformation)
                .Distinct();
        }

        public bool IsNextPolicy(IContentSource source, ITransformerPolicy policy)
        {
            return source.Files.All(x =>
            {
                var policies = _policies[x];
                return policies.Any() && ReferenceEquals(policies.Peek(), policy);
            });
        }

        public void DequeueTransformer(IContentSource source, ITransformerPolicy policy)
        {
            source.Files.Each(x =>
            {
                var policies = _policies[x];
                if (policies.FirstOrDefault() == policy)
                {
                    policies.Dequeue();
                }
            });
        }
    }
}