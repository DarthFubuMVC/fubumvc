using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public interface IMissingAssetHandler
    {
        IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects);
    }

    public class YellowScreenMissingAssetHandler : IMissingAssetHandler
    {
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            throw new NotImplementedException();
        }
    }

    public class TraceOnlyForMissingAssetHandler : IMissingAssetHandler
    {
        // TODO -- trace here!!!
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            throw new NotImplementedException();
        }
    }


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