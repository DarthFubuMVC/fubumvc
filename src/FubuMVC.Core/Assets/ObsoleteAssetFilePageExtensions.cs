using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public static class ObsoleteAssetFilePageExtensions
    {
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
}