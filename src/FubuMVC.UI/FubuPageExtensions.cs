using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;

namespace FubuMVC.UI
{
    public static class FubuPageExtensions
    {
        public static TagGenerator<T> Tags<T>(this IFubuPage<T> page) where T : class
        {
            var generator = page.Get<TagGenerator<T>>();
            generator.Model = page.Model;
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }

        public static TagGenerator<T> Tags<T>(this IFubuPage page) where T : class
        {
            var generator = page.Get<TagGenerator<T>>();
            generator.Model = page.Get<IFubuRequest>().Get<T>();
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }

        public static TagGenerator<T> Tags<T>(this IFubuPage page, T model) where T : class
        {
            var generator = page.Get<TagGenerator<T>>();
            generator.Model = model;
            generator.ElementPrefix = page.ElementPrefix;
            return generator;
        }


        public static HtmlTag LinkTo<TInputModel>(this IFubuPage page) where TInputModel : class, new()
        {
            return page.LinkTo(new TInputModel());
        }

        public static HtmlTag LinkTo(this IFubuPage page, object inputModel)
        {
            return new LinkTag("", page.Urls.UrlFor(inputModel));
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
            string value = page.Model.ValueOrDefault(expression).ToString();
            return new TextboxTag(name, value);
        }

        public static FormTag FormFor(this IFubuPage page)
        {
            return new FormTag();
        }

        public static FormTag FormFor(this IFubuPage page, string url)
        {
            url = UrlContext.GetFullUrl(url);
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
    }
}