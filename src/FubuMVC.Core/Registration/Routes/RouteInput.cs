using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteInput<T> : IRouteInput
    {
        private readonly RouteDefinition _parent;
        private readonly List<RouteParameter> _queryParameters = new List<RouteParameter>();
        private readonly List<RouteParameter> _routeParameters = new List<RouteParameter>();

        public RouteInput(RouteDefinition parent)
        {
            _parent = parent;
        }

        public RouteInput(string pattern) : this(new RouteDefinition(pattern))
        {
            _parent.Input = this;
        }

        public RouteDefinition Parent
        {
            get { return _parent; }
        }

        public List<RouteParameter> RouteParameters
        {
            get { return _routeParameters; }
        }

        public List<RouteParameter> QueryParameters
        {
            get { return _queryParameters; }
        }

        public Type InputType
        {
            get { return typeof(T); }
        }

        public int Rank
        {
            get { return _routeParameters.Count; }
        }

        public string CreateUrlFromInput(object input)
        {
            var url = _parent.Pattern;

            if (_routeParameters.Any(x => !x.CanSubstitue(input)))
            {
                throw new FubuException(
                    2107,
                    "Input model type '{0}' for route '{1}' requires a value for property '{2}', but no value was provided when creating the url and the route definition does not have a default value for this property.",
                    InputType.Name,
                    _parent.Pattern,
                    _routeParameters.First(x => x.DefaultValue == null).Name);
            }

            url = fillRouteValues(url, input);
            url = fillQueryInputs(url, x => x.ToQueryString(input));

            return url;
        }

        public string CreateUrlFromParameters(RouteParameters parameters)
        {
            var url = _parent.Pattern;

            if (parameters == null)
            {
                throw new FubuException(2107, "RouteParameters cannot be null");
            }

            if (_routeParameters.Any(x => !x.IsSatisfied(parameters)))
            {
                throw new FubuException(
                    2107,
                    "Missing required parameters for route {0}, got '{1}'",
                    _parent.Pattern,
                    parameters.AllNames.Join(", "));
            }

            _routeParameters.Each(input => url = input.Substitute(parameters, url));
            url = fillQueryInputs(url, input => input.ToQueryString(parameters));

            return url.ToAbsoluteUrl();
        }

        public string CreateTemplate(object input, Func<object, object>[] hash)
        {
            var url = _parent.Pattern;

            _routeParameters.Where(x => x.CanTemplate(input))
                .Each(r => url = r.Substitute((T)input, url));

            if (hash != null)
                hash.Each(func =>
                {
                    var name = func.Method.GetParameters()[0].Name;
                    var rawValue = func(null);
                    url = url.Replace("{" + name + "}", rawValue.ToString().UrlEncoded());
                });

            url = fillQueryInputs(url, i => i.ToQueryString(input));

            return url.ToAbsoluteUrl();
        }


        public void AlterRoute(Route route)
        {
            var defaults = new RouteValueDictionary();

            _routeParameters.Where(r => r.DefaultValue != null).Each(
                input => defaults.Add(input.Name, input.DefaultValue));

            route.Defaults = defaults;
        }

        private string fillQueryInputs(string url, Func<RouteParameter, string> getQuerystring)
        {
            if (_queryParameters.Count == 0) return url;

            return url + "?" + _queryParameters.Select(getQuerystring).Join("&");
        }

        private string fillRouteValues(string url, object input)
        {
            if (_routeParameters.Count == 0) return url;

            _routeParameters.Each(r => { url = r.Substitute((T)input, url); });

            return url;
        }

        public void AddRouteInput(Expression<Func<T, object>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            var input = new RouteParameter(accessor);

            if (_routeParameters.Any(x => x.Name == input.Name)) return;

            _routeParameters.Add(input);
        }

        public void AddQueryInputs(IEnumerable<RouteParameter> inputs)
        {
            _queryParameters.AddRange(inputs);
        }

        public void AddQueryInput(Expression<Func<T, object>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            var input = new RouteParameter(accessor);

            _queryParameters.Add(input);
        }

        public void AddQueryInput(PropertyInfo property)
        {
            Accessor accessor = new SingleProperty(property);
            var input = new RouteParameter(accessor);

            _queryParameters.Add(input);
        }

        public void AddRouteInput(RouteParameter parameter, bool appendToUrl)
        {
            if (_routeParameters.Any(x => x.Name == parameter.Name)) return;

            if (appendToUrl)
            {
                _parent.Append("{" + parameter.Name + "}");
            }

            _routeParameters.Add(parameter);
        }

        public RouteParameter RouteInputFor(string routeKey)
        {
            return _routeParameters.Single(x => x.Name == routeKey);
        }

        public RouteParameter QueryInputFor(string querystringKey)
        {
            return _queryParameters.Single(x => x.Name == querystringKey);
        }

        public override string ToString()
        {
            return "{0} --> {1}".ToFormat(_parent.Pattern, typeof(T).FullName);
        }
    }
}