using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using FubuCore;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteDefinition : IRouteDefinition
    {
        private readonly RouteValueDictionary _constraints = new RouteValueDictionary();
        private string _pattern;

        public RouteDefinition(string pattern)
        {
            _pattern = pattern;
        }

        public virtual string CreateTemplate(object input, Func<object, object>[] hash)
        {
            return Input == null ? _pattern.ToAbsoluteUrl() : Input.CreateTemplate(input, hash);
        }

        public IRouteInput Input { get; set;}
        public void ApplyInputType(Type inputType)
        {
            Input = RouteBuilder.Build(inputType, Pattern).Input;
        }

        public string CreateUrlFromInput(object input)
        {
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

        public string PatternWithoutDefaultValues
        {
            get
            {
                const string capture = @":\w+";
                var pattern = _pattern;
                return Regex.Matches(pattern, capture, RegexOptions.IgnorePatternWhitespace)
                    .Cast<Match>()
                    .Aggregate(pattern, (current, match) => current.Replace(match.Value, string.Empty));
            }
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

        public IEnumerable<KeyValuePair<string, object>> Constraints
        {
            get { return _constraints; }
        }

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

        protected RouteValueDictionary getConstraints()
        {
            return _constraints.Count > 0 ? _constraints : null;
        }

        public override string ToString()
        {
            // TODO -- need to account for the input?
            return string.Format("{0}", _pattern);
        }
    }
}