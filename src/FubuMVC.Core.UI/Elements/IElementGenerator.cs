using System;
using System.Linq.Expressions;
using HtmlTags;

namespace FubuMVC.Core.UI.Elements
{
    public interface IElementGenerator<T> where T : class
    {
        HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null);
        HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null);
        HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null);

        //ElementRequest GetRequest(Expression<Func<T, object>> expression);
        //ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression);
        
        T Model { get; set; }
        //ILabelAndFieldLayout NewFieldLayout();
    }
}