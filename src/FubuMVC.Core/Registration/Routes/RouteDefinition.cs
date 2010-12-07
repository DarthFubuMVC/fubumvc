using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteDefinition : IRouteDefinition
    {
        private string _pattern;
        private readonly RouteValueDictionary _constraints = new RouteValueDictionary();

        public RouteDefinition(string pattern)
        {
            _pattern = pattern;
        }

        public virtual Type InputType
        {
            get { return null; }
        }

        public virtual string CreateUrl(object input)
        {
            return _pattern.ToAbsoluteUrl();
        }

        public void RootUrlAt(string baseUrl)
        {
            Prepend(baseUrl);
        }

        public virtual Route ToRoute()
        {
            return new Route(_pattern, null, getConstraints(), null);;
        }

        protected RouteValueDictionary getConstraints()
        {
            return _constraints.Count > 0 ? _constraints : null;
        }

        public void Append(string patternPart)
        {
            _pattern += "/" + patternPart;
            _pattern = _pattern.Replace("//", "/").TrimStart('/');
        }

        public string Pattern { get { return _pattern; } }

        public void RemoveLastPatternPart()
        {
            string[] parts = Pattern.Split('/');
            string[] newParts = parts.Take(parts.Length - 1).ToArray();
            _pattern = newParts.Join("/");
        }

        public virtual int Rank
        {
            get { return 0; }
        }

        public IEnumerable<KeyValuePair<string, object>> Constraints { get { return _constraints; } }

        public void AddRouteConstraint(string inputName, IRouteConstraint constraint)
        {
            _constraints[inputName] = constraint;
        }

        public void Prepend(string prefix)
        {
            if (prefix.IsEmpty()) return;

            // Apparently this is necessary
            if (_pattern.StartsWith(prefix)) return;

            _pattern = prefix.TrimEnd('/') + "/" + _pattern;
        }

        public virtual void AddRouteInput(RouteInput input, bool appendToUrl)
        {
            // do nothing
        }

        public virtual void AddQueryInput(PropertyInfo property)
        {
            // do nothing
        }

        public override string ToString()
        {
            return string.Format("{0}", _pattern);
        }
    }

    public class RouteDefinition<T> : RouteDefinition
    {
        private readonly List<RouteInput> _queryInputs = new List<RouteInput>();
        private readonly List<RouteInput> _routeInputs = new List<RouteInput>();

        public RouteDefinition(string pattern)
            : base(pattern)
        {
        }

        public List<RouteInput> RouteInputs { get { return _routeInputs; } }
        public List<RouteInput> QueryInputs { get { return _queryInputs; } }
        public override Type InputType { get { return typeof (T); } }

        public override string CreateUrl(object input)
        {
            string url = Pattern;
            
            if (_routeInputs.Any(x => !x.CanSubstitue(input)))
            {
                throw new FubuException(
                    2107,
                    "Input model type '{0}' for route '{1}' requires a value for property '{2}', but no value was provided when creating the url and the route definition does not have a default value for this property.",
                    InputType.Name,
                    Pattern,
                    _routeInputs.First(x => x.DefaultValue == null).Name);
            }

            url = fillRouteValues(url, input);
            url = fillQueryInputs(url, input);

            return url.ToAbsoluteUrl();
        }


        public override Route ToRoute()
        {
            var defaults = new RouteValueDictionary();

            _routeInputs.Where(r => r.DefaultValue != null).Each(
                input => defaults.Add(input.Name, input.DefaultValue));

            return new Route(Pattern, defaults, getConstraints(), null);
        }

        private string fillQueryInputs(string url, object input)
        {
            if (_queryInputs.Count == 0) return url;

            return url + "?" + _queryInputs.Select(x => x.ToQueryString(input)).Join("&");
        }

        private string fillRouteValues(string url, object input)
        {
            if (_routeInputs.Count == 0) return url;

            _routeInputs.Each(r => { url = r.Substitute((T) input, url); });

            return url;
        }

        public void AddRouteInput(Expression<Func<T, object>> expression)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var input = new RouteInput(accessor);

            if (_routeInputs.Any(x => x.Name == input.Name)) return;

            _routeInputs.Add(input);
        }

        public void AddQueryInputs(IEnumerable<RouteInput> inputs)
        {
            _queryInputs.AddRange(inputs);
        }

        public void AddQueryInput(Expression<Func<T, object>> expression)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var input = new RouteInput(accessor);

            _queryInputs.Add(input);
        }

        public override void AddQueryInput(PropertyInfo property)
        {
            Accessor accessor = new SingleProperty(property);
            var input = new RouteInput(accessor);

            _queryInputs.Add(input);
        }

        public override void AddRouteInput(RouteInput input, bool appendToUrl)
        {
            if (_routeInputs.Any(x => x.Name == input.Name)) return;

            if (appendToUrl)
            {
                Append("{" + input.Name + "}");
            }

            _routeInputs.Add(input);
        }

        public RouteInput RouteInputFor(string routeKey)
        {
            return _routeInputs.Single(x => x.Name == routeKey);
        }

        public RouteInput QueryInputFor(string querystringKey)
        {
            return _queryInputs.Single(x => x.Name == querystringKey);
        }

        public override int Rank
        {
            get
            {
                return _routeInputs.Count;
            }
        }

        public override string ToString()
        {
            return "{0} --> {1}".ToFormat(Pattern, typeof (T).FullName);
        }
    }
}