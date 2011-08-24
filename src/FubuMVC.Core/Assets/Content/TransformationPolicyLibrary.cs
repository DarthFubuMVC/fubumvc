using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Assets.Content
{
    public class TransformationPolicyLibrary
    {
        private readonly IList<ITransformationPolicy> _policies = new List<ITransformationPolicy>();

        public TransformationPolicyLibrary(IEnumerable<ITransformationPolicy> policies)
        {
            _policies.AddRange(policies);
        }

        public IEnumerable<ITransformationPolicy> FindPoliciesFor(AssetFile file)
        {
            var mimeType = file.MimeType;
            return _policies
                .Where(x => x.MimeType == mimeType && x.ActionType != ActionType.Global && x.AppliesTo(file));
        }
        
        public IEnumerable<ITransformationPolicy> FindGlobalPoliciesFor(MimeType mimeType)
        {
            return _policies.Where(x => x.ActionType == ActionType.Global && x.MimeType == mimeType);
        }

        // has all the TransformationPolicy's
        // TODO -- this thing needs to play in the bootstrapping to get the mimetypes
    
    }
}