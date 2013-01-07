using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marks a property as a route input
    /// </summary>
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

    /// <summary>
    /// Marks a property as bound to the querystring so that a non-default
    /// value of the marked property will be added to the generated url
    /// in IUrlRegistry
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringAttribute : Attribute
    {
    }

    // TODO -- change to a ModifyChainAttribute
    /// <summary>
    /// Explicitly specify the url pattern for the chain containing this method
    /// as its ActionCall.  Supports inputs like "folder/find/{Id}"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
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

    /// <summary>
    /// Alters the route generation in the default routing conventions
    /// to override the usual handling of the class name with the value of 
    /// the [UrlFolder] name
    /// </summary>
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

    /// <summary>
    /// Explicitly directs FubuMVC that this endpoint should support output to Json
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonEndpointAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().OutputJson();
        }
    }

    /// <summary>
    /// Explicitly marks this endpoint as asymmetric Json, meaning that it
    /// accepts either form posts or Json posts and outputs json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AsymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeAsymmetricJson();
        }
    }

    /// <summary>
    /// Explicitly applies the selected content negotiation policies and formats
    /// to this endpoint
    /// </summary>
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

            if (_formatters == FormatterOptions.All)
            {
                chain.Input.AllowHttpFormPosts = true;
                chain.UseFormatter<JsonFormatter>();
                chain.UseFormatter<XmlFormatter>();

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
                chain.Input.AllowHttpFormPosts = true;
            }
            else
            {
                chain.Input.AllowHttpFormPosts = false;
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

    /// <summary>
    /// Explicitly marks this endpoint as "symmetric Json," meaning that it
    /// will only accept Json and output to Json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SymmetricJsonAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().MakeSymmetricJson();
        }
    }

    /// <summary>
    ///  This is a marker interface that denotes a Json Endpoint
    /// </summary>
    public interface JsonMessage
    {
    }

    /// <summary>
    /// Explicitly marks this endpoint as returning Html by calling 
    /// the ToString() method on the output as mimetype "text/html"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HtmlEndpointAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            var html = call.ParentChain().Output.AddHtml();
            html.MoveToFront();
        }
    }

    // TODO -- this has to take place before routes
    /// <summary>
    /// Explicitly marks an endpoint as "partial only"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FubuPartialAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCall call)
        {
            call.ParentChain().IsPartialOnly = true;
        }
    }

    /// <summary>
    /// Applies one or more "Wrapper" behaviors of the given types to 
    /// this chain.  The Wrapper nodes are applied immediately before
    /// this ActionCall, from inside to outside
    /// </summary>
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

    /// <summary>
    /// Explicitly adds a category to the Route of this endpoint.  Used to resolve or match
    /// url's or endpoints in usages like IUrlRegistry.UrlFor(model, category)
    /// </summary>
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

    /// <summary>
    /// Marks this endpoint as the creator of the designated type
    /// so that this endpoint will resolve as IUrlRegistry.UrlForNew(type)
    /// </summary>
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

    /// <summary>
    /// FubuMVC applications will treat any assembly marked with the 
    /// [FubuModule] attribute as a Bottle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class FubuModuleAttribute : Attribute
    {
    }
}