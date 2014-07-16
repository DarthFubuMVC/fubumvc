using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Util;
using FubuMVC.Core.Resources.PathBased;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteDefinition : IRouteDefinition, DescribesItself
    {
        public static readonly IEnumerable<string> VERBS = new List<string>
        {
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

        public string Category { get; set; }


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
            if (inputType.CanBeCastTo<ResourcePath>()) return;

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
            get
            {
                if (Input != null && Input.InputType != null && Input.InputType.CanBeCastTo<IRankMeLast>())
                {
                    return int.MaxValue;
                }

                return RouteBuilder.PatternRank(_pattern);
            }
        }

        public Indexer<string, IRouteConstraint> Constraints
        {
            get
            {
                return new Indexer<string, IRouteConstraint>(x => _constraints[x], (key, v) => _constraints[key] = v);
            }
        }

        public void AddHttpMethodConstraint(string method)
        {
            _httpMethods.Fill(method.ToUpper());
        }

        public bool RespondsToMethod(string method)
        {
            return _httpMethods.Any() ? _httpMethods.Contains(method.ToUpper()) : true;
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

        void DescribesItself.Describe(Description description)
        {
            description.Title = "Route:  " + Pattern;
            if (_input != null) description.Children["Input"] = Description.For(_input);

            if (_constraints.Any())
            {
                description.AddList("Constraints", _constraints);
            }

            description.Properties["Http Verbs"] = AllowedHttpMethods.Any() ? AllowedHttpMethods.Join(", ") : "Any";

            description.Properties["SessionStateRequirement"] = SessionStateRequirement == null
                ? "Default"
                : SessionStateRequirement.ToString();
        }


        protected bool Equals(RouteDefinition other)
        {
            return string.Equals(_pattern, other._pattern, StringComparison.OrdinalIgnoreCase)
                   && _constraints.SequenceEqual(other._constraints)
                   && GetHttpMethodConstraints().SequenceEqual(other.GetHttpMethodConstraints());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RouteDefinition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _constraints.GetHashCode();
                hashCode = (hashCode*397) ^ _httpMethods.GetHashCode();
                hashCode = (hashCode*397) ^ _pattern.GetHashCode();
                return hashCode;
            }
        }

        public static IRouteDefinition Empty()
        {
            return new RouteDefinition(string.Empty);
        }
    }
}