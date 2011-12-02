using System;
using System.Collections.Generic;
using FubuCore;
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
            set { DefaultValue = value.ToString(); }
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

        public string Pattern
        {
            get { return _pattern; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UrlFolderAttribute : Attribute
    {
        private readonly string _folder;

        public UrlFolderAttribute(string folder)
        {
            _folder = folder;
        }

        public string Folder
        {
            get { return _folder; }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class JsonEndpointAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().OutputJson();
        }
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


            if ((_formatters & FormatterOptions.Json) != 0)
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
    ///   This is a marker interface that denotes a Json Endpoint
    /// </summary>
    public interface JsonMessage
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HtmlEndpointAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FubuPartialAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().IsPartialOnly = true;
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WrapWithAttribute : ModifyChainAttribute
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

        public override void Alter(ActionCall call)
        {
            _behaviorTypes.Each(x => call.WrapWith(x));
        }
    }


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UrlRegistryCategoryAttribute : ModifyChainAttribute
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

        public override void Alter(ActionCall call)
        {
            call.ParentChain().UrlCategory.Category = Category;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UrlForNewAttribute : ModifyChainAttribute
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

        public override void Alter(ActionCall call)
        {
            call.ParentChain().UrlCategory.Creates.Add(Type);
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class FubuModuleAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class FubuAppAttribute : Attribute
    {
        private Type _type;

        public Type ApplicationSourceType
        {
            get { return _type; }
            set
            {
                if (!value.CanBeCastTo<IApplicationSource>() || !value.IsConcreteWithDefaultCtor())
                {
                    throw new ArgumentOutOfRangeException("ApplicationSourceType",
                                                          "ApplicationSourceType must be a concrete class of IApplicationSource with a no-arg constructor");
                }

                _type = value;
            }
        }
    }
}