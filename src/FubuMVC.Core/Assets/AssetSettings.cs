using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Assets
{
    public class CdnAsset
    {
        public string Url;
        public string Fallback;
        public string File;
    }

    [Description("Allow read access to javascript, css, image, and html files")]
    public class AssetSettings : IStaticFileRule
    {
        public int MaxAgeInSeconds = 24*60*60;

        public AssetSettings()
        {
            if (!FubuMode.InDevelopment())
            {
                var cacheHeader = "private, max-age={0}".ToFormat(MaxAgeInSeconds);
                Headers[HttpResponseHeaders.CacheControl] = () => cacheHeader;
                Headers[HttpResponseHeaders.Expires] = () => DateTime.UtcNow.AddSeconds(MaxAgeInSeconds).ToString("R");
            }
        }

        public readonly IList<CdnAsset> CdnAssets = new List<CdnAsset>(); 

        // This is tested through integration tests
        public static Task Build(BehaviorGraph behaviorGraph)
        {
            return behaviorGraph.Settings.GetTask<AssetSettings>()
                .ContinueWith(t => {
                    var graph = new AssetGraph();
                    var settings = t.Result;

                    var search = settings.CreateAssetSearch();

                    graph.Add(behaviorGraph.Files.FindFiles(search).Select(x => new Asset(x)));

                    settings.Aliases.AllKeys.Each(alias => graph.StoreAlias(alias, settings.Aliases[alias]));

                    settings.CdnAssets.Each(x => graph.RegisterCdnAsset(x));

                    behaviorGraph.Services.AddService<IAssetGraph>(graph);
                });
        }

        public string Exclusions = null;

        public void Exclude(string content)
        {
            Exclusions += string.Empty + ";" + content;
            Exclusions = Exclusions.TrimStart(';');
        }

        public FileSet CreateAssetSearch()
        {
            var extensions = assetMimeTypes().SelectMany(x => x.Extensions).Select(x => "*" + x).Join(";");

            return FileSet.Deep(extensions, Exclusions);
        }

        private IEnumerable<MimeType> assetMimeTypes()
        {
            yield return MimeType.Javascript;
            yield return MimeType.Css;

            foreach (var mimetype in MimeType.All().Where(x => x.Value.StartsWith("image/")))
            {
                yield return mimetype;
            }
        }

        public readonly NameValueCollection Aliases = new NameValueCollection();


        AuthorizationRight IStaticFileRule.IsAllowed(IFubuFile file)
        {
            var mimetype = MimeType.MimeTypeByFileName(file.Path);
            if (mimetype == null) return AuthorizationRight.None;

            if (mimetype == MimeType.Javascript) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Css) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Html) return AuthorizationRight.Allow;

            if (mimetype.Value.StartsWith("image/")) return AuthorizationRight.Allow;

            return AuthorizationRight.None;
        }

        public readonly IList<IStaticFileRule> StaticFileRules
            = new List<IStaticFileRule> {new DenyConfigRule()};

        public AuthorizationRight DetermineStaticFileRights(IFubuFile file)
        {
            return AuthorizationRight.Combine(StaticFileRules.UnionWith(this).Select(x => x.IsAllowed(file)));
        }

        public readonly Cache<string, Func<string>> Headers = new Cache<string, Func<string>>();
    }
}