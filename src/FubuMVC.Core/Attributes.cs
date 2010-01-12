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
}