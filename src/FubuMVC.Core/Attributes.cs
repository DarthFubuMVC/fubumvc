using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media.Formatters;

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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AsymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeAsymmetricJson();
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ConnegAttribute : ModifyChainAttribute
    {
        private FormatterOptions _formatters = FormatterOptions.Html | FormatterOptions.Json | FormatterOptions.Xml;

        public ConnegAttribute() : this(FormatterOptions.All)
        {
        }

        public ConnegAttribute(FormatterOptions formatters)
        {
            _formatters = formatters;
        }

        public FormatterOptions Formatters
        {
            get { return _formatters; }
            set { _formatters = value; }
        }

        public override void Alter(ActionCall call)
        {
            var chain = call.ParentChain();
            chain.ApplyConneg();

            if (_formatters == FormatterOptions.All)
            {
                chain.AlterConnegInput(node => node.AllowHttpFormPosts = true);
                return;
            }



            if ((_formatters & FormatterOptions.Json) != 0 )
            {
                chain.UseFormatter<JsonFormatter>();
            }

            if ((_formatters & FormatterOptions.Xml) != 0)
            {
                chain.UseFormatter<XmlFormatter>();
            }

            if ((_formatters & FormatterOptions.Html) != 0)
            {
                chain.AlterConnegInput(node => node.AllowHttpFormPosts = true);
            }
            else
            {
                chain.AlterConnegInput(node => node.AllowHttpFormPosts = false);
            }
        }
    }

    [Flags]
    public enum FormatterOptions
    {
        Html = 1,
        Json = 2,
        Xml = 4,
        All = 8
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeSymmetricJson();
        }
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