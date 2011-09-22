using System;
using FubuMVC.Core.Behaviors;

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

    // TODO -- change to a ModifyChainAttribute
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

    // TODO -- change to a ModifyChainAttribute
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonEndpointAttribute : Attribute
    {
    }

    /// <summary>
    /// This is a marker interface that denotes a Json Endpoint
    /// </summary>
    public interface JsonMessage{}

    // TODO -- change to a ModifyChainAttribute
    [AttributeUsage(AttributeTargets.Method)]
    public class HtmlEndpointAttribute : Attribute
    {
    }

    // TODO -- change to a ModifyChainAttribute
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

    // TODO -- change to a ModifyChainAttribute
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FubuPartialAttribute : Attribute
    {
    }

    // TODO -- change to a ModifyChainAttribute
    // TODO:  If anyone wants it, make it work on a controller too
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WrapWithAttribute : Attribute
    {
        private readonly Type[] _behaviorTypes;

        public WrapWithAttribute(params Type[] behaviorTypes)
        {
            _behaviorTypes = behaviorTypes;
        }

        public Type[] BehaviorTypes
        {
            get { return _behaviorTypes; }
        }
    }

    // TODO -- change to a ModifyChainAttribute
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

    // TODO -- change to a ModifyChainAttribute
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UrlForNewAttribute : Attribute
    {
        private readonly Type _type;

        public UrlForNewAttribute(Type type)
        {
            _type = type;
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}