using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Forms;
using FubuMVC.UI.Tags;
using HtmlTags;
using FubuMVC.Core.Util;

namespace FubuMVC.UI
{
    public static class FubuPageExtensions
    {
        public static TagGenerator<T> Tags<T>(this IFubuPage<T> page) where T : class
        {
            var generator = page.Get<TagGenerator<T>>();
            generator.Model = page.Model;
            return generator;
        }

        public static void Partial<TInputModel>(this IFubuPage page) where TInputModel : class
        {
            page.Get<IPartialFactory>().BuildPartial(typeof (TInputModel)).InvokePartial();
        }

        public static HtmlTag LinkTo<TInputModel>(this IFubuPage page) where TInputModel : class, new()
        {
            return page.LinkTo(new TInputModel());
        }
        
        public static HtmlTag LinkTo(this IFubuPage page, object inputModel)
        {
            return new LinkTag("", page.Urls.UrlFor(inputModel));
        }

        public static HtmlTag InputFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().InputFor(expression);
        }

        public static HtmlTag LabelFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().LabelFor(expression);
        }

        public static HtmlTag DisplayFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().DisplayFor(expression);
        }

        
        public static string ElementNameFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Get<IElementNamingConvention>().GetName(typeof (T), expression.ToAccessor());
        }

    }
}