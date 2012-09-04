using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteDefinition : TracedNode, IRouteDefinition
    {
        public static readonly IEnumerable<string> VERBS = new List<string>{
            "POST",
            "GET",
            "PUT",
            "DELETE"
        };

        public static readonly string HTTP_METHOD_CONSTRAINT = "HttpMethods";
        private readonly IList<Action<Route>> _alterations = new List<Action<Route>>();

        private readonly Cache<string, IRouteConstraint> _constraints = new Cache<string, IRouteConstraint>();
        private readonly IList<string> _httpMethods = new List<string>();
        private IRouteInput _input;
        private string _pattern;

        public RouteDefinition(string pattern)
        {
            _pattern = pattern;
        }


        public string PatternWithoutDefaultValues
        {
            get
            {
                const string capture = @":\w+";
                var pattern = _pattern;
                var routeUrl = Regex.Matches(pattern, capture, RegexOptions.IgnorePatternWhitespace)
                    .Cast<Match>()
                    .Aggregate(pattern, (current, match) => current.Replace(match.Value, string.Empty));


                return routeUrl;
            }
        }

        public void RegisterRouteCustomization(Action<Route> action)
        {
            _alterations.Add(action);
        }

        public SessionStateRequirement SessionStateRequirement { get; set; }

        public virtual string CreateTemplate(object input, Func<object, object>[] hash)
        {
            return Input == null ? _pattern : Input.CreateTemplate(input, hash);
        }

        public IRouteInput Input
        {
            get { return _input; }
            set
            {
                if (value != null)
                {
                    value.Parent = this;
                }

                _input = value;
            }
        }

        public void ApplyInputType(Type inputType)
        {
            Input = RouteBuilder.Build(inputType, Pattern).Input;
        }

        public string CreateUrlFromInput(object input)
        {
            if (input is IMakeMyOwnUrl)
            {
                return input.As<IMakeMyOwnUrl>().ToUrlPart(Pattern);
            }


            if (input == null)
            {
                return Pattern;
            }

            if (Input == null)
            {
                throw new InvalidOperationException("Cannot call this method if the RouteDefinition has not input type");
            }

            return Input.CreateUrlFromInput(input);
        }

        public void RootUrlAt(string baseUrl)
        {
            Prepend(baseUrl);
        }

        public virtual Route ToRoute()
        {
            var route = new Route(PatternWithoutDefaultValues, null, getConstraints(), null);
            if (Input != null)
            {
                Input.AlterRoute(route);
            }

            _alterations.Each(x => x(route));

            return route;
        }

        public void Append(string patternPart)
        {
            _pattern += "/" + patternPart;
            _pattern = _pattern.Replace("//", "/").TrimStart('/');
        }

        public string Pattern
        {
            get { return _pattern; }
        }

        public void RemoveLastPatternPart()
        {
            var parts = Pattern.Split('/');
            var newParts = parts.Take(parts.Length - 1).ToArray();
            _pattern = newParts.Join("/");
        }

        public virtual int Rank
        {
            get { return Input == null ? 0 : Input.Rank; }
        }

        public Indexer<string, IRouteConstraint> Constraints
        {
            get { return new Indexer<string, IRouteConstraint>(x => _constraints[x], (key, v) => _constraints[key] = v); }
        }

        public void AddHttpMethodConstraint(string method)
        {
            _httpMethods.Fill(method.ToUpper());
        }

        public bool RespondsToGet()
        {
            return _httpMethods.Any() ? _httpMethods.Contains("GET") : true;
        }

        public IList<string> AllowedHttpMethods
        {
            get { return _httpMethods; }
        }

        public void Prepend(string prefix)
        {
            if (prefix.IsEmpty()) return;

            // Apparently this is necessary
            if (_pattern.StartsWith(prefix)) return;

            _pattern = prefix.TrimEnd('/') + "/" + _pattern;
        }

        public IEnumerable<string> GetHttpMethodConstraints()
        {
            return _httpMethods.OrderBy(x => x);
        }

        protected RouteValueDictionary getConstraints()
        {
            var dictionary = new RouteValueDictionary();
            if (_httpMethods.Any())
            {
                dictionary.Add(HTTP_METHOD_CONSTRAINT, new HttpMethodConstraint(_httpMethods.ToArray()));
            }

            _constraints.Each(dictionary.Add);

            return dictionary.Any() ? dictionary : null;
        }

        public override string ToString()
        {
            // TODO -- need to account for the input?
            return string.Format("{0}", _pattern);
        }

        public static IRouteDefinition Empty()
        {
            return new RouteDefinition(string.Empty);
        }
    }
}