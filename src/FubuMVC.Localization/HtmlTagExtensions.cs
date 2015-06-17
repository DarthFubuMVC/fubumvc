using System;
using System.Linq.Expressions;
using FubuLocalization;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Localization
{
    public static class HtmlTagExtensions
    {
        public static HtmlTag Text(this HtmlTag tag, StringToken token)
        {
            return tag.Text(token == null ? string.Empty : token.ToString());
        }

        public static HtmlTag Attr(this HtmlTag tag, string attName, StringToken token)
        {
            return tag.Attr(attName, token.ToString());
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
    }
}