using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class Asset
    {
        public readonly IFubuFile File;
        public readonly string Url;
        public readonly MimeType MimeType;
        public readonly string Filename;

        // For testing only!
        public Asset(string url)
        {
            Url = url;
            Filename = Path.GetFileName(url);
        }

        public Asset(IFubuFile file)
        {
            File = file;

            Filename = Path.GetFileName(file.Path);
            MimeType = MimeType.MimeTypeByFileName(Filename);

            Url = file.RelativePath.Replace("\\", "/").TrimStart('/');
        }
    }

    public class AssetSettings
    {
        public Task<AssetGraph> Build(IFubuApplicationFiles files)
        {
            return Task.Factory.StartNew(() => {
                var graph = new AssetGraph();

                var search = CreateAssetSearch();

                graph.Add(files.FindFiles(search).Select(x => new Asset(x)));


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
    }

    public interface IAssetGraph
    {
        Asset FindAsset(string search);
        IEnumerable<Asset> Assets { get; }
    }

    public class AssetGraph : IAssetGraph
    {
        private readonly IList<Asset> _assets = new List<Asset>();
        private readonly ConcurrentCache<string, Asset> _searches = new ConcurrentCache<string, Asset>();

        public AssetGraph()
        {
            _searches.OnMissing = findAsset;
        }

        public void Add(Asset asset)
        {
            _assets.Add(asset);
        }

        public void Add(IEnumerable<Asset> assets)
        {
            _assets.AddRange(assets);
        }

        public Asset FindAsset(string search)
        {
            return _searches[search];
        }

        private Asset findAsset(string search)
        {
            search = search.TrimStart('/');

            var exact = _assets.FirstOrDefault(x => x.Url == search);
            if (exact != null) return exact;

            if (search.Contains("/"))
            {
                var pathSearch = "/" + search;
                return _assets.FirstOrDefault(x => x.Url.EndsWith(pathSearch));
            }

            return _assets.FirstOrDefault(x => x.Filename == search);
        }

        public IEnumerable<Asset> Assets
        {
            get { return _assets; }
        }
    }

    public interface IAssetTagBuilder
    {
        IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts);
        IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> scripts);
    }

    public class DevelopmentModeAssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetGraph _graph;
        private AssetTagBuilder _inner;

        public DevelopmentModeAssetTagBuilder(IAssetGraph graph)
        {
            _graph = graph;

            _inner = new AssetTagBuilder(graph);
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> scripts)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetGraph _graph;

        public AssetTagBuilder(IAssetGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            return scripts.Select(x => {
                var asset = _graph.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new ScriptTag(url);
            });
        }

        

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> stylesheets)
        {
            return stylesheets.Select(x =>
            {
                var asset = _graph.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new StylesheetLinkTag(url);
            });
        }
    }

    public static class AssetFilePageExtensions
    {
        /// <summary>
        ///   Renders an HTML img tag to display the specified file from the application's image folder
        /// </summary>
        /// <param name = "viewPage"></param>
        /// <param name = "imageFilename">The name of the image file, relative to the applications' image folder</param>
        /// <returns></returns>
        public static ImageTag Image(this IFubuPage viewPage, string imageFilename)
        {
            string imageUrl = viewPage.ImageUrl(imageFilename);
            return new ImageTag(imageUrl);
        }

        /// <summary>
        ///   Returns the absolute URL for an image (may be a customer overridden path or a package image path)
        /// </summary>
        /// <param name = "viewPage"></param>
        /// <param name = "imageFilename">The name of the image file, relative to the applications' image folder</param>
        /// <returns></returns>
        public static string ImageUrl(this IFubuPage viewPage, string imageFilename)
        {
            var asset = viewPage.Get<IAssetGraph>().FindAsset(imageFilename);
            // TODO -- asset null handling in dev mode

            return asset.Url;
        }

        /// <summary>
        /// Writes an <image> tag for the named image using the url from the asset pipeline
        /// *Same* functionality as Image()
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static HtmlTag ImageFor(this IFubuPage page, string assetName)
        {
            return page.Image(assetName);
        }



        /// <summary>
        /// Writes out &lt;script&gt; tags for each script
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static TagList Script(this IFubuPage page, params string[] scripts)
        {
            return page.Get<IAssetTagBuilder>().BuildScriptTags(scripts).ToTagList();
        }


        /// <summary>
        /// Writes out &lt;link&gt; tags for each stylesheet
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cssNames"></param>
        public static TagList Css(this IFubuPage page, params string[] cssNames)
        {
            return page.Get<IAssetTagBuilder>().BuildStylesheetTags(cssNames).ToTagList();
        }


        /// <summary>
        /// Registers an asset for the current request *if* it can be found.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static void OptionalScript(this IFubuPage page, params string[] scripts)
        {
            throw new NotImplementedException();
            //page.OptionalAsset(scripts);
        }


        /// <summary>
        /// OBSOLETE, only here to provide guidance on new usage
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        [Obsolete("This is deprecated and will throw an exception.  Use Css() or Script() instead")]
        public static TagList Asset(this IFubuPage page, params string[] assets)
        {
            throw new NotSupportedException("The IFubuPage.Asset() method is no longer functional in 2.0.  Use IFubuPage.Css() and/or IFubuPage.Script() instead");
        }


        /// <summary>
        /// OBSOLETE, only here to provide guidance on new usage
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        [Obsolete("This method is deprecated and will throw an exception. Use OptionalCss() or OptionalScript() instead")]
        public static void OptionalAsset(this IFubuPage page, params string[] assets)
        {
            throw new NotSupportedException("The IFubuPage.OptionalAsset() method is no longer functional in 2.0. Use IFubuPage.OptionalScript() or IFubuPage.OptionalCss() instead");
        }


        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Script() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scriptNames"></param>
        /// <returns></returns
        [Obsolete("Removed in 2.0.  Use IFubuPage.Script() instead")]
        public static TagList WriteScriptTags(this IFubuPage page, params string[] scriptNames)
        {
            throw new NotSupportedException("Removed in 2.0.  Use IFubuPage.Script() instead");
        }

        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assetNames"></param>
        /// <returns></returns>
        [Obsolete("Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead")]
        public static TagList WriteAssetTags(this IFubuPage page, params string[] assetNames)
        {
            throw new NotSupportedException("Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead");
        }

        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mimeType"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        [Obsolete("Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead")]
        public static TagList WriteAssetTagsImmediately(this IFubuPage page, MimeType mimeType, params string[] names)
        {
            throw new NotSupportedException("Removed in 2.0.  Use IFubuPage.Script() or IFubuPage.Css() instead");
        }

        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Script() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        [Obsolete("Removed in 2.0.  Use IFubuPage.Script() instead")]
        public static TagList WriteScriptTagsImmediately(this IFubuPage page, params string[] names)
        {
            throw new NotSupportedException("Removed in 2.0.  Use IFubuPage.Script() instead");
        }
        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Css() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        [Obsolete("Removed in 2.0.  Use IFubuPage.Css() instead")]
        public static TagList WriteCssTagsImmediately(this IFubuPage page, params string[] names)
        {
            throw new NotSupportedException("WriteCssTagsImmediately() is obsolete.  Just use the Css() method instead");
        }



        /// <summary>
        /// OBSOLETE, Removed in 2.0.  Use IFubuPage.Css() instead
        /// </summary>
        /// <param name="page"></param>
        /// <param name="styleNames"></param>
        /// <returns></returns>
        [Obsolete("Removed in 2.0.  Use IFubuPage.Css() instead")]
        public static TagList WriteCssTags(this IFubuPage page, params string[] styleNames)
        {
            throw new NotSupportedException("WriteCssTags() is obsolete.  Just use the Css() method instead");
        }
    }

    public class ScriptTag : HtmlTag
    {
        public ScriptTag(string url)
            : base("script")
        {
            // http://stackoverflow.com/a/1288319/75194 
            Attr("type", "text/javascript");
            Attr("src", url);
        }
    }

    public class StylesheetLinkTag : HtmlTag
    {
        public StylesheetLinkTag(string url)
            : base("link")
        {
            Attr("href", url);
            Attr("rel", "stylesheet");
            Attr("type", MimeType.Css.Value);
        }
    }

}