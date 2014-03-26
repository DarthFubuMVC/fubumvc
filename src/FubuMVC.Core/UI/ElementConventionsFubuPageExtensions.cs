using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class ElementConventionsFubuPageExtensions
    {
        public static IElementGenerator<T> Tags<T>(this IFubuPage<T> page) where T : class
        {
            return page.Get<IElementGenerator<T>>();
        }

        /// <summary>
        ///   Builds a tag that accepts user input for a property of the page's view model
        /// </summary>
        /// <typeparam name = "T">The model type of the strongly typed view</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Tags().InputFor(expression, model: page.Model);
        }

        /// <summary>
        ///   Builds a tag that accepts user input for a property on a model retrieved from the <see cref = "IFubuRequest" />
        /// </summary>
        /// <typeparam name = "T">The type to retrieve from the request</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Get<IElementGenerator<T>>().InputFor(expression);
        }

        /// <summary>
        ///   Builds a tag that accepts user input tag for a property on a provided model instance
        /// </summary>
        /// <typeparam name = "T">The type of the given model</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "model">The model used to provide values for the tag</param>
        /// <param name = "expression">An expression that specifies a property on the provided model</param>
        /// <returns></returns>
        public static HtmlTag InputFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementGenerator<T>>().InputFor(expression, model: model);
        }


        /// <summary>
        ///   Builds a tag that displays the name of a property on the page's view model
        /// </summary>
        /// <typeparam name = "T">The model type of the strongly typed view</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementGenerator<T>>().LabelFor(expression, model:page.Model);
        }

        /// <summary>
        ///   Builds a tag that displays the name of a property on a model retrieved from the <see cref = "IFubuRequest" />
        /// </summary>
        /// <typeparam name = "T">The type to retrieve from the request</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Get<IElementGenerator<T>>().LabelFor(expression);
        }


        /// <summary>
        ///   Builds a tag that displays the name of a property on a model retrieved from the <see cref = "IFubuRequest" />
        /// </summary>
        /// <typeparam name = "T">The type to retrieve from the request</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "model">The model used to provide values for the tag</param>
        /// <param name = "expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag LabelFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression)
           where T : class
        {
            return page.Get<IElementGenerator<T>>().LabelFor(expression, model: model);
        }

        /// <summary>
        ///   Builds a tag that displays the current value of a property on the page's view model
        /// </summary>
        /// <typeparam name = "T">The model type of the strongly typed view</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the model</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementGenerator<T>>().DisplayFor(expression, model: page.Model);
        }

        /// <summary>
        ///   Builds a tag that displays the current value of a property on a model retrieved from the <see cref = "IFubuRequest" />
        /// </summary>
        /// <typeparam name = "T">The type to retrieve from the request</typeparam>
        /// <param name = "page">The view</param>
        /// <param name = "expression">An expression that specifies a property on the retrieved type instance</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage page, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementGenerator<T>>().DisplayFor(expression);
        }

        /// <summary>
        ///   Builds a tag that displays the current value of a property on a given model
        /// </summary>
        /// <typeparam name = "T">The type of the given model</typeparam>
        /// <param name = "page">The view</param>2
        /// <param name = "model">The model used to provide values for the tag</param>
        /// <param name = "expression">An expression that specifies a property on the provided model</param>
        /// <returns></returns>
        public static HtmlTag DisplayFor<T>(this IFubuPage page, T model, Expression<Func<T, object>> expression)
            where T : class
        {
            return page.Get<IElementGenerator<T>>().DisplayFor(expression, model:model);
        }
    }
}