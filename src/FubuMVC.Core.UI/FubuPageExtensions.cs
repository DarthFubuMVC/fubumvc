using System;
using System.Linq.Expressions;
using FubuLocalization;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.View;
using HtmlTags;
using HtmlTags.Extended.Attributes;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI
{
    public static class FubuPageExtensions
    {
        /// <summary>
        /// Writes an <image> tag for the named assetName using the url from the asset pipeline
        /// </summary>
        /// <param name="page"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static HtmlTag ImageFor(this IFubuPage page, string assetName)
        {
            string url = page.Get<IAssetUrls>().UrlForAsset(AssetFolder.images, assetName);
            return new HtmlTag("img").Attr("src", url);
        }


        /// <summary>
        ///   Just returns the localized header text for a property of the view model
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "page"></param>
        /// <param name = "expression"></param>
        /// <returns></returns>
        public static string HeaderText<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return LocalizationManager.GetHeader(expression);
        }

        /// <summary>
        /// Generic html helper to create an authorization aware link.  Typically, you would only use
        /// this method as a helper for more specific html helpers
        /// </summary>
        /// <param name="page"></param>
        /// <param name="finder"></param>
        /// <returns></returns>
        public static HtmlTag AuthorizedLinkTo(this IFubuPage page, Func<IEndpointService, Endpoint> finder)
        {
            var endpoints = page.Get<IEndpointService>();
            Endpoint endpoint = finder(endpoints);

            return new LinkTag(string.Empty, endpoint.Url)
                .Authorized(endpoint.IsAuthorized);
        }

        /// <summary>
        /// Creates a link tag for the endpoint that accepts type T as the input.  This method is authorization aware.
        /// </summary>
        /// <typeparam name="TInputModel"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static HtmlTag LinkTo<TInputModel>(this IFubuPage page) where TInputModel : class, new()
        {
            return page.LinkTo(new TInputModel());
        }

        /// <summary>
        /// Creates a link tag for the input message.  This handler is authorization aware.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public static HtmlTag LinkTo(this IFubuPage page, object inputModel)
        {
            if (inputModel == null)
            {
                return new HtmlTag("a").Authorized(false);
            }

            return page.AuthorizedLinkTo(x => x.EndpointFor(inputModel));
        }
        
        /// <summary>
        /// Creates a link tag pointing to the controller/endpoint method.  Nothing is output
        /// if the user is not authorized to view this resource
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="page"></param>
        /// <param name="actionExpression"></param>
        /// <returns></returns>
        public static HtmlTag LinkTo<TController>(this IFubuPage page, Expression<Action<TController>> actionExpression)
        {
            return page.AuthorizedLinkTo(x => x.EndpointFor(actionExpression));
        }

        /// <summary>
        /// Creates a link tag pointing to the endpoint that is marked as 
        /// creating type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <returns></returns>
        public static HtmlTag LinkToNew<T>(this IFubuPage page)
        {
            return page.AuthorizedLinkTo(x => x.EndpointForNew<T>());
        }

        /// <summary>
        /// Generates a json variable to the url for the "input" object like:
        /// var [variable] = '[IUrlRegistry.UrlFor(input)]';
        /// </summary>
        /// <param name="page"></param>
        /// <param name="variable"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LinkVariable(this IFubuPage page, string variable, object input)
        {
            string url = page.Urls.UrlFor(input);
            return "var {0} = '{1}';".ToFormat(variable, url);
        }

        /// <summary>
        /// Creates a json variable to the url for the input type T
        /// var [variable] = '[IUrlRegistry.UrlFor<T>()]'
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="page"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string LinkVariable<TInput>(this IFubuPage page, string variable) where TInput : new()
        {
            return page.LinkVariable(variable, new TInput());
        }

 
        /// <summary>
        /// Exercises the IElementNamingConvention to determine the element name for an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ElementNameFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementNamingConvention>().GetName(typeof (T), expression.ToAccessor());
        }

        /// <summary>
        /// Explicit helper to generate an <input type="text"></input> html element for 
        /// a property on the current view model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TextboxTag TextBoxFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            string name = ElementNameFor(page, expression);
            var value = page.Model.ValueOrDefault(expression);
            return new TextboxTag(name, (value == null) ? "" : value.ToString());
        }

        /// <summary>
        /// Generates an html element like:  <span>[text]</span>
        /// </summary>
        /// <param name="page"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HtmlTag Span(this IFubuPage page, string text)
        {
            return new HtmlTag("span").Text(text);
        }


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
            return viewPage.Get<IAssetUrls>().UrlForAsset(AssetFolder.images, imageFilename);
        }


    }
}