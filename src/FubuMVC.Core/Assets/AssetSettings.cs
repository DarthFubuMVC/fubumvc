using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Packaging;
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

    public enum SearchMode
    {
        /// <summary>
        /// Use assets from any location in either the application or loaded Bottles
        /// </summary>
        Anywhere,

        /// <summary>
        /// Limits the asset sourcing to the files under the PublicFolder/[Version] folder
        /// </summary>
        PublicFolderOnly
    }

    


    [Description("Allow read access to javascript, css, image, and html files")]
    public class AssetSettings : IStaticFileRule
    {
        /// <summary>
        /// The default maximum age in seconds to cache an asset in production mode. 1 day.
        /// </summary>
        public int MaxAgeInSeconds = 24*60*60;



        public AssetSettings()
        {
            if (!FubuMode.InDevelopment())
            {
                var cacheHeader = "private, max-age={0}".ToFormat(MaxAgeInSeconds);
                Headers[HttpResponseHeaders.CacheControl] = () => cacheHeader;
                Headers[HttpResponseHeaders.Expires] = () => DateTime.UtcNow.AddSeconds(MaxAgeInSeconds).ToString("R");
            }

            Exclude("node_modules/*");

            Mode = SearchMode.Anywhere;
            PublicFolder = "public";

            TemplateDestination = "_templates";

        }

        internal string PublicAssetFolder ;

        /// <summary>
        /// Add assets that will be sourced by CDN
        /// </summary>
        public readonly IList<CdnAsset> CdnAssets = new List<CdnAsset>(); 

        // This is tested through integration tests
        public static Task Build(BehaviorGraph behaviorGraph)
        {
            return behaviorGraph.Settings.GetTask<AssetSettings>()
                .RecordContinuation("Building the Asset Graph", t => {
                    var graph = new AssetGraph();
                    var settings = t.Result;

                    var files = findAssetFiles(behaviorGraph, settings);

                    graph.Add(files.Select(x => new Asset(x)));

                    settings.Aliases.AllKeys.Each(alias => graph.StoreAlias(alias, settings.Aliases[alias]));

                    settings.CdnAssets.Each(x => graph.RegisterCdnAsset(x));

                    behaviorGraph.Services.AddService<IAssetGraph>(graph);
                });
        }

        /// <summary>
        /// Used internally to determine the public folder if the mode is set to
        /// PublicFolderOnly
        /// </summary>
        /// <returns></returns>
        public string DeterminePublicFolder()
        {
            var candidate = FubuMvcPackageFacility.GetApplicationPath().AppendPath(PublicFolder);

            if (Version.IsNotEmpty())
            {
                var versioned = candidate.AppendPath(Version);
                if (Directory.Exists(versioned))
                {
                    candidate = versioned;
                }
            }

            if (!Directory.Exists(candidate))
            {
                Console.WriteLine("The designated public asset folder '{0}' cannot be found".ToFormat(candidate));
            }

            return candidate;
        }

        
        

        private static IEnumerable<IFubuFile> findAssetFiles(BehaviorGraph behaviorGraph, AssetSettings settings)
        {
            var search = settings.CreateAssetSearch();

            if (settings.Mode == SearchMode.PublicFolderOnly)
            {
                var publicFolder = settings.DeterminePublicFolder();
                var appFolder = FubuMvcPackageFacility.GetApplicationPath();

                return new FileSystem().FindFiles(publicFolder, search)
                    .Select(x => {
                        return new FubuFile(x, ContentFolder.Application)
                        {
                            RelativePath = x.PathRelativeTo(appFolder)
                        };
                    });
            }


            return behaviorGraph.Files.FindFiles(search);
        }

        public string Exclusions = null;

        /// <summary>
        /// Exclude a file by name or an entire sub folder with the syntax '[folder]/*'
        /// 'node_modules' is excluded by default
        /// </summary>
        /// <param name="content"></param>
        public void Exclude(string content)
        {
            Exclusions += string.Empty + ";" + content;
            Exclusions = Exclusions.TrimStart(';');
        }

        /// <summary>
        /// Used internally to build up the file search for assets
        /// </summary>
        /// <returns></returns>
        public FileSet CreateAssetSearch()
        {
            var extensions = assetMimeTypes().SelectMany(x => x.Extensions).Union(AllowableExtensions).Select(x => "*" + x).Join(";");

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

        /// <summary>
        /// Add name aliases for assets like "jquery" = "jquery.1.18.min.js"
        /// </summary>
        public readonly NameValueCollection Aliases = new NameValueCollection();


        AuthorizationRight IStaticFileRule.IsAllowed(IFubuFile file)
        {
            var mimetype = MimeType.MimeTypeByFileName(file.Path);
            if (mimetype == null) return AuthorizationRight.None;

            if (mimetype == MimeType.Javascript) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Css) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Html) return AuthorizationRight.Allow;

            if (mimetype.Value.StartsWith("image/")) return AuthorizationRight.Allow;

            if (AllowableExtensions.Contains(Path.GetExtension(file.Path))) return AuthorizationRight.Allow;

            return AuthorizationRight.None;
        }

        /// <summary>
        /// Configure more allowable static files if you need to customize what
        /// files are allowed to be served via http
        /// </summary>
        public readonly IList<IStaticFileRule> StaticFileRules
            = new List<IStaticFileRule> {new DenyConfigRule()};

        public AuthorizationRight DetermineStaticFileRights(IFubuFile file)
        {
            return AuthorizationRight.Combine(StaticFileRules.UnionWith(this).Select(x => x.IsAllowed(file)));
        }

        /// <summary>
        /// The Http headers to be written when serving up static files
        /// </summary>
        public readonly Cache<string, Func<string>> Headers = new Cache<string, Func<string>>();


        /// <summary>
        /// Default is 'public'. Establishes the public folder if you are publishing all assets to one folder at build time
        /// </summary>
        public string PublicFolder { get; set; }

        /// <summary>
        /// Defines the published version of the assets and uses this string to find the public asset folder. Is ignored if the [public]/[version] folder does not exist. Default is null.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Switch between serving assets from anywhere and assets from only the public folder. Default is Anywhere.
        /// </summary>
        public SearchMode Mode { get; set; }

        /// <summary>
        /// Add additional file extensions as allowable assets
        /// </summary>
        public IList<string> AllowableExtensions = new List<string>{".eot", ".ttf", ".woff", ".svg"};


        /// <summary>
        /// The path relative to the application where generated html templates should be 
        /// written
        /// </summary>
        public string TemplateDestination { get; set; }

        /// <summary>
        /// Designate all the CultureInfo's for template generation. Default is ['en-US']
        /// </summary>
        public IList<string> TemplateCultures = new List<string>{"en-US"}; 
    }
}