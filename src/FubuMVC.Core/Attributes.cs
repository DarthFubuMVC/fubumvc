using System;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RouteInputAttribute : Attribute
    {
        public RouteInputAttribute()
        {
        }

        public RouteInputAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultObject
        {
            set
            {
                DefaultValue = value.ToString();
            }
        }

        public string DefaultValue { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UrlPatternAttribute : Attribute
    {
        private readonly string _pattern;

        public UrlPatternAttribute(string pattern)
        {
            _pattern = pattern.Trim();
        }

        public string Pattern { get { return _pattern; } }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UrlFolderAttribute : Attribute
    {
        private readonly string _folder;

        public UrlFolderAttribute(string folder)
        {
            _folder = folder;
        }

        public string Folder { get { return _folder; } }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class JsonEndpointAttribute : Attribute
    {
    }

    /// <summary>
    /// This is a marker interface that denotes a Json Endpoint
    /// </summary>
    public interface JsonMessage{}

    [AttributeUsage(AttributeTargets.Method)]
    public class HtmlEndpointAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class WebFormsEndpointAttribute : Attribute
    {
        private readonly Type _viewType;

        public WebFormsEndpointAttribute(Type viewType)
        {
            _viewType = viewType;
        }

        public Type ViewType { get { return _viewType; } }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FubuPartialAttribute : Attribute
    {
    }

    /// <summary>
    /// Just declares 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UrlRegistryCategoryAttribute : Attribute
    {
        private readonly string _category;

        public UrlRegistryCategoryAttribute(string category)
        {
            _category = category;
        }

        public string Category
        {
            get { return _category; }
        }
    }
}