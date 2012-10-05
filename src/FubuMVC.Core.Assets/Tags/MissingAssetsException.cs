using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuMVC.Core.Assets.Tags
{
    [Serializable]
    public class MissingAssetsException : Exception
    {
        public MissingAssetsException(IEnumerable<MissingAssetTagSubject> subjects)
            : base("Requested assets {0} cannot be found".ToFormat(subjects.Select(x => x.Name).Join(", ")))
        {
        }

        protected MissingAssetsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}