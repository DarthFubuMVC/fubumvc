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
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Assets
{
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

        // This is tested through integration tests
        public Task<AssetGraph> Build(IFubuApplicationFiles files)
        {
            return Task.Factory.StartNew(() => {
                var graph = new AssetGraph();

                var search = CreateAssetSearch();

                graph.Add(files.FindFiles(search).Select(x => new Asset(x)));

                Aliases.AllKeys.Each(alias => graph.StoreAlias(alias, Aliases[alias]));

                return graph;
            });
        }

        public FileSet CreateAssetSearch()
        {
            var extensions = assetMimeTypes().SelectMany(x => x.Extensions).Select(x => "*" + x).Join(";");

            return FileSet.Deep(extensions);
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