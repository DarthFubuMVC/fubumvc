using System;
using System.Linq;
using FubuMVC.Core.Runtime;
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
        /// Registers one or more assets and their dependencies as required, but does
        /// NOT write out any html tags
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        public static void Asset(this IFubuPage page, params string[] assets)
        {
            

            throw new NotImplementedException();
            //page.Get<IAssetRequirements>().Require(assets);
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
        /// Registers an asset for the current request *if* it can be found.  Does NOT
        /// write the html tags
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        public static void OptionalAsset(this IFubuPage page, params string[] assets)
        {
            throw new NotImplementedException();
            //page.Get<IAssetRequirements>().UseAssetIfExists(assets);
        }


        /// <summary>
        /// Registers all the specified style assets, then writes html tags for *only*
        /// the assets where the MimeType is CSS
        /// </summary>
        /// <param name="page"></param>
        /// <param name="styleNames"></param>
        /// <returns></returns>
        public static TagList WriteCssTags(this IFubuPage page, params string[] styleNames)
        {
            throw new NotImplementedException();
            //page.Asset(styleNames);
            //return page.Get<IAssetTagWriter>().WriteTags(MimeType.Css);
        }

        /// <summary>
        /// Registers all the specified script assets, then writes html tags for *only*
        /// the assets where the MimeType is Javascript
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scriptNames"></param>
        /// <returns></returns>
        public static TagList WriteScriptTags(this IFubuPage page, params string[] scriptNames)
        {
            throw new NotImplementedException();
            page.Asset(scriptNames);
            //return page.Get<IAssetTagWriter>().WriteTags(MimeType.Javascript);
        }

        /// <summary>
        /// Registers all the specified assets as required *and* writes the html tags
        /// for all known required assets and their dependencies
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assetNames"></param>
        /// <returns></returns>
        public static TagList WriteAssetTags(this IFubuPage page, params string[] assetNames)
        {
            throw new NotImplementedException();
            page.Asset(assetNames);
            //return page.Get<IAssetTagWriter>().WriteAllTags();
        }

        /// <summary>
        /// Write the tags immediately and dequeue them from being rendered again.
        /// Useful when *always* want jquery at the top
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mimeType"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static TagList WriteAssetTagsImmediately(this IFubuPage page, MimeType mimeType, params string[] names)
        {
            throw new NotImplementedException();
            //var correctedNames = page.Get<AssetGraph>().CorrectForAliases(names).ToArray();
            //return page.Get<IAssetTagWriter>().WriteTags(mimeType, correctedNames);
        }

        /// <summary>
        /// Write the tags immediately and dequeue them from being rendered again.
        /// Useful when *always* want jquery at the top
        /// </summary>
        /// <param name="page"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static TagList WriteScriptTagsImmediately(this IFubuPage page, params string[] names)
        {
            throw new NotImplementedException();
            //var correctedNames = page.Get<AssetGraph>().CorrectForAliases(names).ToArray();

            //return page.Get<IAssetTagWriter>().WriteTags(MimeType.Javascript, correctedNames);
        }
        /// <summary>
        /// Write the tags immediately and dequeue them from being rendered again.
        /// Useful when *always* want jquery at the top 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static TagList WriteCssTagsImmediately(this IFubuPage page, params string[] names)
        {
            throw new NotImplementedException();
            //var correctedNames = page.Get<AssetGraph>().CorrectForAliases(names).ToArray();
            //return page.Get<IAssetTagWriter>().WriteTags(MimeType.Css, correctedNames);
        }
    }

    public class ScriptTag : HtmlTag
    {
        public ScriptTag(string url) : base("script")
        {
            // http://stackoverflow.com/a/1288319/75194 
            Attr("type", "text/javascript");
            Attr("src", url);
        }
    }

    public class StylesheetLinkTag : HtmlTag
    {
        public StylesheetLinkTag(string url) : base("link")
        {
            Attr("href", url);
            Attr("rel", "stylesheet");
            Attr("type", MimeType.Css.Value);
        }
    }

}