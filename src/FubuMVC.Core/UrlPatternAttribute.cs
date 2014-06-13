using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core
{

    public class GetAttribute : UrlPatternAttribute
    {
        public GetAttribute(string pattern) : base(pattern)
        {
        }

        public override IRouteDefinition BuildRoute(Type inputType)
        {
            var route = base.BuildRoute(inputType);
            route.AddHttpMethodConstraint("GET");

            return route;
        }
    }

    public class PostAttribute : UrlPatternAttribute
    {
        public PostAttribute(string pattern)
            : base(pattern)
        {
        }

        public override IRouteDefinition BuildRoute(Type inputType)
        {
            var route = base.BuildRoute(inputType);
            route.AddHttpMethodConstraint("POST");

            return route;
        }
    }

    public class PutAttribute : UrlPatternAttribute
    {
        public PutAttribute(string pattern)
            : base(pattern)
        {
        }

        public override IRouteDefinition BuildRoute(Type inputType)
        {
            var route = base.BuildRoute(inputType);
            route.AddHttpMethodConstraint("PUT");

            return route;
        }
    }

    public class DeleteAttribute : UrlPatternAttribute
    {
        public DeleteAttribute(string pattern)
            : base(pattern)
        {
        }

        public override IRouteDefinition BuildRoute(Type inputType)
        {
            var route = base.BuildRoute(inputType);
            route.AddHttpMethodConstraint("DELETE");

            return route;
        }
    }

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

        public virtual IRouteDefinition BuildRoute(Type inputType)
        {
            var index = _pattern.IndexOf("::");
            var pattern = index == -1 ? _pattern : _pattern.Substring(index + 2, _pattern.Length - index -2);

            var route = inputType == null
                ? new RouteDefinition(pattern)
                : RouteBuilder.Build(inputType, pattern);

            if (index > -1)
            {
                _pattern.Substring(0, index).ToDelimitedArray(',').Each(route.AddHttpMethodConstraint);
            }

            return route;
        }
    }
}