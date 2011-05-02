using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.View;
using FubuMVC.WebForms.Partials;

namespace FubuMVC.WebForms
{
    public static class PartialExpressionExtensions
    {
        public static RenderPartialExpression<TInputModel> PartialForEach<TInputModel, TPartialModel>(
            this IFubuPage<TInputModel> page, Expression<Func<TInputModel, IEnumerable<TPartialModel>>> listExpression)
            where TInputModel : class
            where TPartialModel : class
        {
            var expression = new RenderPartialExpression<TInputModel>(page.Model, page, page.Get<IPartialRenderer>(), page.Tags(), page.Get<IEndpointService>())
                .ForEachOf(listExpression, page.ElementPrefix);

            SearchPartialView<TInputModel, TPartialModel>(page, expression);
            return expression;
        }

        private static void SearchPartialView<TInputModel, TPartialModel>(IFubuPage<TInputModel> page, RenderPartialExpression<TInputModel> expression) where TInputModel : class
        {
            var renderer = page.ServiceLocator.GetInstance<IPartialViewTypeRegistry>();
            if (renderer.HasPartialViewTypeFor<TPartialModel>())
                expression.Using(renderer.GetPartialViewTypeFor<TPartialModel>());
        }


        /// <summary>
        /// Renders a partial view for a sequence of items on the view model
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model</typeparam>
        /// <typeparam name="TPartialViewModel">The type of the items in the sequence</typeparam>
        /// <param name="viewPage"></param>
        /// <param name="expression">An expression that retrieves the sequence from the view model</param>
        /// <returns></returns>
        public static RenderPartialExpression<TViewModel> RenderPartialForEachOf
            <TViewModel, TPartialViewModel>(this IFubuPage<TViewModel> viewPage,
                                            Expression<Func<TViewModel, IEnumerable<TPartialViewModel>>> expression)
            where TViewModel : class
            where TPartialViewModel : class
        {
            var renderer = viewPage.Get<IPartialRenderer>();
            return
                new RenderPartialExpression<TViewModel>(viewPage.Model, viewPage, renderer, viewPage.Tags(), viewPage.Get<IEndpointService>()).ForEachOf(
                    expression);
        }

        /// <summary>
        /// Renders a partial view for a sequence of untyped items
        /// </summary>
        /// <param name="page"></param>
        /// <param name="items">The seqence of items</param>
        /// <returns></returns>
        public static RenderPartialExpression<T> RenderPartialForEachOf<T>(
            this IFubuPage page, IEnumerable<T> items) where T : class
        {
            var renderer = page.Get<IPartialRenderer>();
            return new RenderPartialExpression<T>(null, page, renderer, page.Get<ITagGenerator<T>>(), page.Get<IEndpointService>()).ForEachOf(items);
        }

        /// <summary>
        /// Renders a partial view for an object from the view model
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model</typeparam>
        /// <typeparam name="TPartialViewModel">The type of property from the view model</typeparam>
        /// <param name="viewPage"></param>
        /// <param name="expression">An expression that retrieves the property value from the view model</param>
        /// <returns></returns>
        public static RenderPartialExpression<TViewModel> RenderPartialFor
            <TViewModel, TPartialViewModel>(
            this IFubuPage<TViewModel> viewPage, Expression<Func<TViewModel, TPartialViewModel>> expression)
            where TViewModel : class
            where TPartialViewModel : class
        {
            var renderer = viewPage.Get<IPartialRenderer>();
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var partialModel = accessor.GetValue(viewPage.Model) as TPartialViewModel;

            return
                new RenderPartialExpression<TViewModel>(viewPage.Model, viewPage, renderer, viewPage.Tags(), viewPage.Get<IEndpointService>()).For(
                    partialModel);
        }

        /// <summary>
        /// Renders a partial view for the view model
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model</typeparam>
        /// <param name="viewPage"></param>
        /// <returns></returns>
        public static RenderPartialExpression<TViewModel> RenderPartial<TViewModel>(
            this IFubuPage<TViewModel> viewPage) where TViewModel : class
        {
            var renderer = viewPage.Get<IPartialRenderer>();
            return
                new RenderPartialExpression<TViewModel>(viewPage.Model, viewPage, renderer, viewPage.Tags(), viewPage.Get<IEndpointService>()).For(
                    viewPage.Model);
        }

        /// <summary>
        /// Renders a partial view for the view model
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model</typeparam>
        /// <param name="viewPage"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static RenderPartialExpression<TViewModel> RenderPartialFor<TViewModel>(
            this IFubuPage viewPage, TViewModel model) where TViewModel : class
        {
            var renderer = viewPage.Get<IPartialRenderer>();
            return
                new RenderPartialExpression<TViewModel>(model, viewPage, renderer, viewPage.Tags<TViewModel>(), viewPage.Get<IEndpointService>()).For(
                    model);
        }



    }
}