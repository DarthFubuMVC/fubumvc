using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Content;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using HtmlTags;
using HtmlTags.Extended.Attributes;

namespace FubuMVC.Core.UI
{
    public static class FubuPageExtensions
    {
        public static ITagGenerator<T> Tags<T>(this IFubuPage<T> page) where T : class
        {
            var generator = page.Get<ITagGenerator<T>>();
            generator.Model = page.Model ?? page.Get<IFubuRequest>().Get<T>();
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }

        public static ITagGenerator<T> Tags<T>(this IFubuPage page) where T : class
        {
            var generator = page.Get<ITagGenerator<T>>();
            generator.Model = page.Get<IFubuRequest>().Get<T>();
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }

        public static ITagGenerator<T> Tags<T>(this IFubuPage page, T model) where T : class
        {
            var generator = page.Get<ITagGenerator<T>>();
            generator.Model = model;
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }

        public static HtmlTag AuthorizedLinkTo(this IFubuPage page, Func<IEndpointService, Endpoint> finder)
        {
            var endpoints = page.Get<IEndpointService>();
            var endpoint = finder(endpoints);

            return new LinkTag(string.Empty, endpoint.Url)
                .Authorized(endpoint.IsAuthorized);
        }


        public static HtmlTag LinkTo<TInputModel>(this IFubuPage page) where TInputModel : class, new()
        {
            return page.LinkTo(new TInputModel());
        }

        public static HtmlTag LinkTo(this IFubuPage page, object inputModel)
        {
            if (inputModel == null)
            {
                return new HtmlTag("a").Authorized(false);
            }

            return page.AuthorizedLinkTo(x => x.EndpointFor(inputModel));
        }

        public static HtmlTag LinkTo<TController>(this IFubuPage page, Expression<Action<TController>> actionExpression)
        {
            return page.AuthorizedLinkTo(x => x.EndpointFor(actionExpression));
        }

        public static HtmlTag LinkToNew<T> (this IFubuPage page)
        {
            return page.AuthorizedLinkTo(x => x.EndpointForNew<T>());
        }
        
        public static string LinkVariable(this IFubuPage page, string variable, object input)
        {
            string url = page.Urls.UrlFor(input);
            return "var {0} = '{1}';".ToFormat(variable, url);
        }

        public static string LinkVariable<TInput>(this IFubuPage page, string variable) where TInput : new()
        {
            return page.LinkVariable(variable, new TInput());
        }

        /// <summary>
        /// Builds a tag that accepts user input for a property of the page's view model
        /// </summary>
        /// <typeparam name="T">The model type of the strongly typed view</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags().InputFor(expression);
        }

        /// <summary>
        /// Builds a tag that accepts user input for a property on a model retrieved from the <see cref="IFubuRequest" />
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the request</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags<T>().InputFor(expression);
        }

        /// <summary>
        /// Builds a tag that accepts user input tag for a property on a provided model instance
        /// </summary>
        /// <typeparam name="T">The type of the given model</typeparam>
        /// <param name="page">The view</param>
        /// <param name="model">The model used to provide values for the tag</param>
        /// <param name="expression">An expression that specifies a property on the provided model</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags(model).InputFor(expression);
        }


        /// <summary>
        /// Builds a tag that displays the name of a property on the page's view model
        /// </summary>
        /// <typeparam name="T">The model type of the strongly typed view</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags().LabelFor(expression);
        }

        /// <summary>
        /// Builds a tag that displays the name of a property on a model retrieved from the <see cref="IFubuRequest" />
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the request</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags<T>().LabelFor(expression);
        }

        /// <summary>
        /// Builds a tag that displays the name of a property on a given model
        /// </summary>
        /// <typeparam name="T">The type of the given model</typeparam>
        /// <param name="page">The view</param>
        /// <param name="model">The model used to provide values for the tag</param>
        /// <param name="expression">An expression that specifies a property on the provided model</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags(model).LabelFor(expression);
        }

        /// <summary>
        /// Builds a tag that displays the current value of a property on the page's view model
        /// </summary>
        /// <typeparam name="T">The model type of the strongly typed view</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags().DisplayFor(expression);
        }

        /// <summary>
        /// Builds a tag that displays the current value of a property on a model retrieved from the <see cref="IFubuRequest" />
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the request</typeparam>
        /// <param name="page">The view</param>
        /// <param name="expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags<T>().DisplayFor(expression);
        }

        /// <summary>
        /// Builds a tag that displays the current value of a property on a given model
        /// </summary>
        /// <typeparam name="T">The type of the given model</typeparam>
        /// <param name="page">The view</param>
        /// <param name="model">The model used to provide values for the tag</param>
        /// <param name="expression">An expression that specifies a property on the provided model</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags(model).DisplayFor(expression);
        }

        public static string ElementNameFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementNamingConvention>().GetName(typeof (T), expression.ToAccessor());
        }

        public static TextboxTag TextBoxFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            string name = ElementNameFor(page, expression);
			object value = page.Model.ValueOrDefault(expression);
			return new TextboxTag(name, (value == null) ? "" : value.ToString());
        }

        // TODO -- Jeremy to add tests
        // IN Dovetail, want to add a label attribute for the localized header of the property
        public static CheckboxTag CheckBoxFor<T>(this IFubuPage<T> page, Expression<Func<T, bool>> expression) where T : class
        {
            // TODO -- run modifications on this?
            return new CheckboxTag(page.Model.ValueOrDefault(expression));
        }

        public static FormTag FormFor(this IFubuPage page)
        {
            return new FormTag();
        }

        public static FormTag FormFor(this IFubuPage page, string url)
        {
            url = url.ToAbsoluteUrl();
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page) where TInputModel : new()
        {
            string url = page.Urls.UrlFor(new TInputModel());
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page, TInputModel model)
        {
            string url = page.Urls.UrlFor(model);
            return new FormTag(url);
        }


        public static FormTag FormFor<TController>(this IFubuPage view, Expression<Action<TController>> expression)
        {
            string url = view.Urls.UrlFor(expression);
            return new FormTag(url);
        }


        public static FormTag FormFor(this IFubuPage view, object modelOrUrl)
        {
            string url = modelOrUrl as string ?? view.Urls.UrlFor(modelOrUrl);

            return new FormTag(url);
        }

        public static string EndForm(this IFubuPage page)
        {
            return "</form>";
        }

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
            var imageUrl = viewPage.ImageUrl(imageFilename);
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
            return viewPage.Get<IContentRegistry>().ImageUrl(imageFilename);
        }

        public static HtmlTag AntiForgeryToken(this IFubuPage page, string salt)
        {
            return AntiForgeryToken(page, salt, null, null);
        }

        public static HtmlTag AntiForgeryToken(this IFubuPage page, string salt, string path, string domain)
        {
            var antiForgeryService = page.Get<IAntiForgeryService>();
            var cookieToken = antiForgeryService.SetCookieToken(path, domain);
            var formToken = antiForgeryService.GetFormToken(cookieToken, salt);

            return new HiddenTag().Name(formToken.Name).Value(formToken.TokenString);
        }
    }
}