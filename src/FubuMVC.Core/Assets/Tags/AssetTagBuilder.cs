using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>
            _builders = new Cache<MimeType, Func<IAssetTagSubject, string, HtmlTag>>();

        private readonly IMissingAssetHandler _missingHandler;
        private readonly IAssetUrls _urls;

        public AssetTagBuilder(IMissingAssetHandler missingHandler, IAssetUrls urls)
        {
            _missingHandler = missingHandler;
            _urls = urls;

            _builders[MimeType.Javascript] = (subject, url) => {
                return new HtmlTag("script")
                    // http://stackoverflow.com/a/1288319/75194 
                    .Attr("type", "text/javascript")
                    .Attr("src", url);
            };

            _builders[MimeType.Css] =
                (subject, url) => {
                    return new HtmlTag("link").Attr("href", url).Attr("rel", "stylesheet").Attr("type",
                                                                                                MimeType.Css.Value);
                };
        }

        public IEnumerable<HtmlTag> Build(AssetTagPlan plan)
        {
            // This will happen when a user tries to request an asset set
            // with no assets -- think optional sets
            if (!plan.Subjects.Any())
            {
                return new HtmlTag[0];
            }

            IEnumerable<MissingAssetTagSubject> missingSubjects = plan.RemoveMissingAssets();
            Func<IAssetTagSubject, string, HtmlTag> func = _builders[plan.MimeType];
            Func<IAssetTagSubject, HtmlTag> builder = s => {
                string url = _urls.UrlForAsset(s.Folder, s.Name);
                return func(s, url);
            };

            IEnumerable<HtmlTag> missingTags = _missingHandler.BuildTagsAndRecord(missingSubjects);
            IEnumerable<HtmlTag> assetTags = plan.Subjects.Select(builder);
            return missingTags.Union(assetTags);
        }
    }
}