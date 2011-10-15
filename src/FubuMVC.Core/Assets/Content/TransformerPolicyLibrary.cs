using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Assets.Content
{
    public class TransformerPolicyLibrary : ITransformerPolicyLibrary
    {
        private readonly IList<ITransformerPolicy> _policies = new List<ITransformerPolicy>();

        public TransformerPolicyLibrary(IEnumerable<ITransformerPolicy> policies)
        {
            _policies.AddRange(policies);
        }

        public IEnumerable<ITransformerPolicy> FindPoliciesFor(AssetFile file)
        {
            var mimeType = file.MimeType;
            var policies = _policies
                .Where(x => x.MimeType == mimeType && x.ActionType != ActionType.Global && x.AppliesTo(file))
                .ToList();

            policies.Sort(new TransformerComparer(file));

            return policies;
        }
        
        // At least the order would be predictable here
        public IEnumerable<ITransformerPolicy> FindGlobalPoliciesFor(MimeType mimeType)
        {
            return _policies.Where(x => x.ActionType == ActionType.Global && x.MimeType == mimeType);
        }

    
    }
}