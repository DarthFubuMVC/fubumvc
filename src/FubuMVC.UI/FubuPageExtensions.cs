using System;
using System.Linq.Expressions;
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