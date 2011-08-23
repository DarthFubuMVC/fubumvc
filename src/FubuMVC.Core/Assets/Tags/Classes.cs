using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class AssetTagWriter
    {
        private readonly Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>
            _builders = new Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>();

        private readonly IMissingAssetHandler _missingHandler;

        public AssetTagWriter(IMissingAssetHandler missingHandler)
        {
            _missingHandler = missingHandler;

            _builders[MimeType.Javascript] = (subject, url) =>
            {
                return new HtmlTag("script")
                    .Attr("type", MimeType.Javascript.Value)
                    .Attr("src", url);
            };

            _builders[MimeType.Css] = (subject, url) =>
            {
                return new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type", MimeType.Css.Value);
            };
        }

        public IEnumerable<HtmlTag> Write(AssetTagPlan plan)
        {
            throw new NotImplementedException();

            var missingSubjects = plan.RemoveMissingAssets();
            var func = _builders[plan.MimeType];
            Func<IAssetTagSubject, HtmlTag> builder = s =>
            {
                var url = AssetFileHandler.DetermineAssetUrl(s);
                return func(s, url);
            };

            var missingTags = _missingHandler.BuildTagsAndRecord(missingSubjects);
            var assetTags = plan.Subjects.Select(builder);
            return missingTags.Union(assetTags); 
        }
    }


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