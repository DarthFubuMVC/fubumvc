using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Content
{
    public interface ITransformerPolicyLibrary
    {
        IEnumerable<ITransformerPolicy> FindPoliciesFor(AssetFile file);
        IEnumerable<ITransformerPolicy> FindGlobalPoliciesFor(MimeType mimeType);
    }
}