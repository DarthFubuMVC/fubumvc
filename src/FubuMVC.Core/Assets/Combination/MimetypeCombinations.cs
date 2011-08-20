using System;
using FubuCore.Util;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class MimetypeCombinations : Cache<string, CombinationCandidate>
    {
        public MimetypeCombinations(MimeType mimeType) : base(name => new CombinationCandidate(mimeType, name))
        {
        }
    }
}