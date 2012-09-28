using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using HtmlTags;
using HtmlTags.Conventions;
using FubuCore.Reflection;

namespace FubuMVC.Core.UI.Elements
{
    public class ElementGenerator<T> : IElementGenerator<T> where T : class
    {
        private readonly ITagGenerator<ElementRequest> _tags;
        private Lazy<T> _model;

        public ElementGenerator(ITagGenerator<ElementRequest> tags, IFubuRequest request)
        {
            _tags = tags;
            _model = new Lazy<T>(request.Get<T>);
        }

        public ElementRequest GetRequest(Expression<Func<T, object>> expression)
        {
            return new ElementRequest(expression.ToAccessor()){
                Model = Model
            };
        }

        private HtmlTag build(Expression<Func<T, object>> expression, string category, string profile = null)
        {
            var request = GetRequest(expression);
            return _tags.Build(request, category, profile);
        }


        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile = null)
        {
            return build(expression, ElementConstants.Label, profile);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile = null)
        {
            return build(expression, ElementConstants.Editor, profile);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile = null)
        {
            return build(expression, ElementConstants.Display, profile);
        }

        public T Model
        {
            get { return _model.Value; }
            set { _model = new Lazy<T>(() => value); }
        }
    }
}