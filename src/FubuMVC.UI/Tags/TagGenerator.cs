using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using FubuMVC.UI.Configuration;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.UI.Tags
{
    public interface ITagGenerator<T> where T : class
    {
        void SetProfile(string profileName);
        HtmlTag LabelFor(Expression<Func<T, object>> expression);
        HtmlTag LabelFor(Expression<Func<T, object>> expression, string profile);
        HtmlTag InputFor(Expression<Func<T, object>> expression);
        HtmlTag InputFor(Expression<Func<T, object>> expression, string profile);
        
        
        HtmlTag DisplayFor(Expression<Func<T, object>> expression);
        HtmlTag DisplayFor(Expression<Func<T, object>> expression, string profile);
        ElementRequest GetRequest(Expression<Func<T, object>> expression);
        HtmlTag LabelFor(ElementRequest request);
        HtmlTag InputFor(ElementRequest request);
        HtmlTag DisplayFor(ElementRequest request);
        ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression);
        string ElementPrefix { get; set; }
        string CurrentProfile { get; }
        T Model { get; set; }
        ElementRequest GetRequest(Accessor accessor);
    }

    public class TagGenerator<T> : ITagGenerator<T> where T : class
    {
        private readonly TagProfileLibrary _library;
        private T _model;
        private readonly IElementNamingConvention _namingConvention;
        private readonly IServiceLocator _services;
        private readonly Stringifier _stringifier;
        private TagProfile _profile;


        public TagGenerator(TagProfileLibrary library, IElementNamingConvention namingConvention, IServiceLocator services, Stringifier stringifier)
        {
            ElementPrefix = string.Empty;
            
            _library = library;
            _namingConvention = namingConvention;
            _services = services;
            _stringifier = stringifier;

            _profile = _library.DefaultProfile;
        }

        public T Model { get { return _model; } set { _model = value; } }

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
            var request = new ElementRequest(_model, accessor, _services, _stringifier);
            determineElementName(request);
            return request;
        }

        public ElementRequest GetRequest<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var request = new ElementRequest(_model, accessor, _services, _stringifier);
            determineElementName(request);
            return request;
        }

        private void determineElementName(ElementRequest request)
        {
            var prefix = ElementPrefix ?? string.Empty;
            request.ElementId = prefix + _namingConvention.GetName(typeof(T), request.Accessor);
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