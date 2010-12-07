using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Tags;
using HtmlTags;

namespace FubuMVC.Tests.UI
{
    public class StubTagGenerator<T> : ITagGenerator<T> where T : class
    {
        public void SetModel(object model) {}

        public void SetProfile(string profileName)
        {
            throw new NotImplementedException();
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression)
        {
            return new HtmlTag("span").AddClass("label").Text(expression.ToAccessor().Name);
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile)
        {
            throw new NotImplementedException();
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression)
        {
            return new HtmlTag("span").AddClass("input").Text(expression.ToAccessor().Name);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile)
        {
            throw new NotImplementedException();
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression)
        {
            return new HtmlTag("span").AddClass("display").Text(expression.ToAccessor().Name);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile)
        {
            throw new NotImplementedException();
        }

        public ElementRequest GetRequest(Expression<Func<T, object>> expression)
        {
            return new ElementRequest(Activator.CreateInstance(typeof(T)), expression.ToAccessor(), null);
        }

        public HtmlTag LabelFor(ElementRequest request)
        {
            return new HtmlTag("span").AddClass("label").Text(request.Accessor.Name);
        }

        public HtmlTag InputFor(ElementRequest request)
        {
            return new HtmlTag("span").AddClass("input").Text(request.Accessor.Name);
        }

        public HtmlTag DisplayFor(ElementRequest request)
        {
            return new HtmlTag("span").AddClass("display").Text(request.Accessor.Name);
        }

        public ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public string ElementPrefix { get { throw new NotImplementedException(); }
            set { } }

        public string CurrentProfile
        {
            get { throw new NotImplementedException(); } }

        public T Model { get { throw new NotImplementedException(); }
            set {  } }

        public ILabelAndFieldLayout NewFieldLayout()
        {
            return new DefinitionListLabelAndField();
        }

        public ElementRequest GetRequest(Accessor accessor)
        {
            throw new NotImplementedException();
        }

        public HtmlTag BeforePartial(ElementRequest request)
        {
            throw new NotImplementedException();
        }

        public HtmlTag AfterPartial(ElementRequest request)
        {
            throw new NotImplementedException();
        }

        public HtmlTag AfterEachofPartial(ElementRequest request, int current, int count)
        {
            throw new NotImplementedException();
        }

        public HtmlTag BeforeEachofPartial(ElementRequest request, int current, int count)
        {
            throw new NotImplementedException();
        }
    }
}