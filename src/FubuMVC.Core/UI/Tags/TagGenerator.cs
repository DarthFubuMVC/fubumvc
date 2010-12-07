using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Forms;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.UI.Tags
{
    public interface ITagGenerator
    {
        void SetModel(object model);
        void SetProfile(string profileName);
        HtmlTag LabelFor(ElementRequest request);
        HtmlTag InputFor(ElementRequest request);
        HtmlTag DisplayFor(ElementRequest request);
        string ElementPrefix { get; set; }
        string CurrentProfile { get; }
        ElementRequest GetRequest(Accessor accessor);

        HtmlTag BeforePartial(ElementRequest request);
        HtmlTag AfterPartial(ElementRequest request);
        HtmlTag AfterEachofPartial(ElementRequest request, int current, int count);
        HtmlTag BeforeEachofPartial(ElementRequest request, int current, int count);
    }

    public interface ITagGenerator<T> : ITagGenerator where T : class
    {
        HtmlTag LabelFor(Expression<Func<T, object>> expression);
        HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile);
        HtmlTag InputFor(Expression<Func<T, object>> expression);
        HtmlTag InputFor(Expression<Func<T, object>> expression, string profile);
        
        
        HtmlTag DisplayFor(Expression<Func<T, object>> expression);
        HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile);
        ElementRequest GetRequest(Expression<Func<T, object>> expression);
        ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression);
        T Model { get; set; }
        ILabelAndFieldLayout NewFieldLayout();
    }

    public class TagGenerator<T> : ITagGenerator<T> where T : class
    {
        private readonly TagProfileLibrary _library;
        private T _model;
        private readonly IElementNamingConvention _namingConvention;
        private readonly IServiceLocator _services;
        private TagProfile _profile;


        public TagGenerator(TagProfileLibrary library, IElementNamingConvention namingConvention, IServiceLocator services)
        {
            ElementPrefix = string.Empty;
            
            _library = library;
            _namingConvention = namingConvention;
            _services = services;

            _profile = _library.DefaultProfile;
        }

        public T Model { get { return _model; } set { _model = value; } }

        public void SetModel(object model)
        {
            Model = (T) model;
        }

        public void SetProfile(string profileName)
        {
            _profile = _library[profileName];
        }

        public string CurrentProfile
        {
            get
            {
                return _profile.Name;
            }
        }

        private HtmlTag buildTag(Expression<Func<T, object>> expression, TagFactory factory)
        {
            ElementRequest request = GetRequest(expression);

            return factory.Build(request);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile)
        {
            return buildTag(expression, _library[profile].Display);
        }

        public string ElementPrefix { get; set; }

        public ElementRequest GetRequest(Expression<Func<T, object>> expression)
        {
            Accessor accessor = expression.ToAccessor();
            return GetRequest(accessor);
        }

        public ElementRequest GetRequest(Accessor accessor)
        {
            var request = new ElementRequest(_model, accessor, _services);
            determineElementName(request);
            return request;
        }

        public HtmlTag BeforePartial(ElementRequest request)
        {
            return _profile.BeforePartial.Build(request);
        }

        public HtmlTag AfterPartial(ElementRequest request)
        {
            return _profile.AfterPartial.Build(request);
        }

        public HtmlTag AfterEachofPartial(ElementRequest request, int current, int count)
        {
            return _profile.AfterEachOfPartial.Build(request, current, count);
        }

        public HtmlTag BeforeEachofPartial(ElementRequest request, int current, int count)
        {
            return _profile.BeforeEachOfPartial.Build(request, current, count);
        }

        public ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var request = new ElementRequest(_model, accessor, _services);
            determineElementName(request);
            return request;
        }

        private void determineElementName(ElementRequest request)
        {
            // TODO -- this is a klooge because of the tag wrapping around partials
            if (request.Accessor == null) return;

            var prefix = ElementPrefix ?? string.Empty;
            request.ElementId = prefix + _namingConvention.GetName(typeof(T), request.Accessor);
        }

        public ILabelAndFieldLayout NewFieldLayout()
        {
            return _profile.NewLabelAndFieldLayout();
        }

        public HtmlTag LabelFor(ElementRequest request)
        {
            return _profile.Label.Build(request);
        }

        public HtmlTag InputFor(ElementRequest request)
        {
            return _profile.Editor.Build(request);
        }

        public HtmlTag DisplayFor(ElementRequest request)
        {
            return _profile.Display.Build(request);
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression)
        {
            return buildTag(expression, _profile.Label);
        }

        public HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile)
        {
            return buildTag(expression, _library[profile].Label);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression)
        {
            return buildTag(expression, _profile.Editor);
        }

        public HtmlTag InputFor(Expression<Func<T, object>> expression, string profile)
        {
            return buildTag(expression, _library[profile].Editor);
        }

        public HtmlTag DisplayFor(Expression<Func<T, object>> expression)
        {
            return buildTag(expression, _profile.Display);
        }
    }
}