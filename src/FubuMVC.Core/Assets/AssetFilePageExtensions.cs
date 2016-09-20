﻿using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public static class AssetFilePageExtensions
    {
        /// <summary>
        ///   Creates an HTML img tag to display the specified file from the application's image folder
        /// </summary>
        /// <param name = "viewPage"></param>
        /// <param name = "fileOrUrl">The name of the image file, relative to the applications' image folder</param>
        /// <returns></returns>
        public static ImageTag Image(this IFubuPage viewPage, string fileOrUrl)
        {
            var imageUrl = Uri.IsWellFormedUriString(fileOrUrl, UriKind.Absolute)
                ? fileOrUrl
                : viewPage.ImageUrl(fileOrUrl);

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
        /// Creates an <image> tag for the named image using the url from the asset pipeline
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
        /// Creates &lt;script&gt; tags for each script
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static TagList Script(this IFubuPage page, params string[] scripts)
        {
            return page.Get<IAssetTagBuilder>().BuildScriptTags(scripts).ToArray().ToTagList();
        }


        /// <summary>
        /// Creates &lt;link&gt; tags for each stylesheet
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cssNames"></param>
        public static TagList Css(this IFubuPage page, params string[] cssNames)
        {
            return page.Get<IAssetTagBuilder>().BuildStylesheetTags(cssNames).ToArray().ToTagList();
        }


        /// <summary>
        /// Creates a script tag if the named scripts can be found.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static TagList OptionalScript(this IFubuPage page, params string[] scripts)
        {
            var finder = page.Get<IAssetFinder>();
            var request = page.Get<IHttpRequest>();
            var mode = page.Get<FubuRuntime>().Mode;

            var tags = scripts.Select(finder.FindAsset).Where(x => x != null)
                .Select(x => new ScriptTag(mode, request.ToFullUrl, x)).ToArray();

            return new TagList(tags);
        }


        /// <summary>
        /// Creates link tags for the named stylesheets if they exist
        /// </summary>
        /// <param name="page"></param>
        /// <param name="stylesheets"></param>
        public static TagList OptionalCss(this IFubuPage page, params string[] stylesheets)
        {
            var finder = page.Get<IAssetFinder>();
            var request = page.Get<IHttpRequest>();
            var tags = stylesheets.Select(finder.FindAsset).Where(x => x != null)
                .Select(x => new StylesheetLinkTag(request.ToFullUrl(x.Url))).ToArray();

            return new TagList(tags);
        }

        /// <summary>
        /// "Require" script assets to be written later.  Useful to register scripts in partial
        /// views that need to be written by the master layout
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static void RequireScript(this IFubuPage page, params string[] scripts)
        {
            page.Get<IAssetTagBuilder>().RequireScript(scripts);
        }

        public static HtmlTag WriteJavascriptRoutes(this JavascriptRouteWriter writer, string groupName,
            IJavascriptRouter router)
        {
            var dict = writer.Write(router.Routes());

            var tag = new HtmlTag("script").Attr("type", "text/javascript");
            tag.Encoded(false);

            tag.Text("\n{0} = {1};\n".ToFormat(groupName, JsonUtil.ToJson(dict)));

            return tag;
        }

        public static HtmlTag JavascriptRoutes<T>(this IFubuPage page, string groupName)
            where T : IJavascriptRouter, new()
        {
            var writer = page.Get<JavascriptRouteWriter>();
            var router = new T();

            return writer.WriteJavascriptRoutes(groupName, router);
        }

        public static string PublicAssetFolder(this IFubuPage page)
        {
            var settings = page.Get<AssetSettings>();
            var files = page.Get<IFubuApplicationFiles>();

            if (settings.PublicAssetFolder.IsEmpty())
            {
                var directory = settings.DeterminePublicFolder(files);
                var relativePath = directory.PathRelativeTo(files.RootPath)
                    .Replace('\\', '/');

                settings.PublicAssetFolder = page.Get<IHttpRequest>().ToFullUrl(relativePath);
            }

            return settings.PublicAssetFolder;
        }
    }
}