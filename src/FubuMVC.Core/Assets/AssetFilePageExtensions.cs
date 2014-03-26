using System;
using System.Linq;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public static class AssetFilePageExtensions
    {
        /// <summary>
        ///   Renders an HTML img tag to display the specified file from the application's image folder
        /// </summary>
        /// <param name = "viewPage"></param>
        /// <param name = "fileOrUrl">The name of the image file, relative to the applications' image folder</param>
        /// <returns></returns>
        public static ImageTag Image(this IFubuPage viewPage, string fileOrUrl)
        {
            string imageUrl = Uri.IsWellFormedUriString(fileOrUrl, UriKind.Absolute) ? fileOrUrl : viewPage.ImageUrl(fileOrUrl);
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
            return viewPage.Get<IAssetTagBuilder>().FindImageUrl(imageFilename);
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
        /// Writes out a script tag if the named scripts can be found.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static TagList OptionalScript(this IFubuPage page, params string[] scripts)
        {
            var graph = page.Get<IAssetGraph>();
            var tags = scripts.Select(graph.FindAsset).Where(x => x != null)
                .Select(x => new ScriptTag(x)).ToArray();

            return new TagList(tags);

        }


        /// <summary>
        /// Writes out link tags for the named stylesheets if they exist
        /// </summary>
        /// <param name="page"></param>
        /// <param name="stylesheets"></param>
        public static TagList OptionalCss(this IFubuPage page, params string[] stylesheets)
        {
            var graph = page.Get<IAssetGraph>();
            var tags = stylesheets.Select(graph.FindAsset).Where(x => x != null)
                .Select(x => new StylesheetLinkTag(x.Url)).ToArray();

            return new TagList(tags);
        }
    }
}