using System;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class AssetFilePageExtensions
    {
        /// <summary>
        /// Registers one or more scripts and their dependencies as required assets,
        /// but does NOT write out any html tags.
        /// Really just a call to page.Asset(), but left here for backwards compatibility
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static void Script(this IFubuPage page, params string[] scripts)
        {
            page.Asset(scripts);
        }

        /// <summary>
        /// Registers one or more assets and their dependencies as required, but does
        /// NOT write out any html tags
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        public static void Asset(this IFubuPage page, params string[] assets)
        {
            page.Get<IAssetRequirements>().Require(assets);
        }

        /// <summary>
        /// Registers an asset for the current request *if* it can be found.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="scripts"></param>
        public static void OptionalScript(this IFubuPage page, params string[] scripts)
        {
            page.OptionalAsset(scripts);
        }
        
        /// <summary>
        /// Registers an asset for the current request *if* it can be found.  Does NOT
        /// write the html tags
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assets"></param>
        public static void OptionalAsset(this IFubuPage page, params string[] assets)
        {
            page.Get<IAssetRequirements>().UseAssetIfExists(assets);
        }

        /// <summary>
        /// Registers one or more style assets and their dependencies as required assets,
        /// but does NOT write out any html tags.
        /// Really just a call to page.Asset(), but left here for backwards compatibility
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cssNames"></param>
        public static void Css(this IFubuPage page, params string[] cssNames)
        {
            page.Asset(cssNames);
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
            page.Asset(styleNames);
            return page.Get<IAssetTagWriter>().WriteTags(MimeType.Css);
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
            page.Asset(scriptNames);
            return page.Get<IAssetTagWriter>().WriteTags(MimeType.Javascript);
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
            page.Asset(assetNames);
            return page.Get<IAssetTagWriter>().WriteAllTags();
        }

        
    }
}