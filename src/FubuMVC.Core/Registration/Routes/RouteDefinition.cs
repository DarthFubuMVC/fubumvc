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



    public class RouteDefinition : IRouteDefinition
    {
        private readonly RouteValueDictionary _constraints = new RouteValueDictionary();
        private string _pattern;

        public RouteDefinition(string pattern)
        {
            _pattern = pattern;
        }

        public virtual Type InputType
        {
            get { return null; }
        }

        public virtual string CreateUrlFromInput(object input)
        {
            return _pattern.ToAbsoluteUrl();
        }

        public virtual string CreateUrlFromParameters(RouteParameters parameters)
        {
            throw new NotSupportedException();
        }

        public virtual string CreateTemplate(object input, Func<object, object>[] hash)
        {
            return _pattern.ToAbsoluteUrl();
        }

        public void RootUrlAt(string baseUrl)
        {
            Prepend(baseUrl);
        }

        public virtual Route ToRoute()
        {
            return new Route(_pattern, null, getConstraints(), null);
            ;
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
            get { return 0; }
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

        public virtual void AddRouteInput(RouteParameter parameter, bool appendToUrl)
        {
            // do nothing
        }

        public virtual void AddQueryInput(PropertyInfo property)
        {
            // do nothing
        }

        protected RouteValueDictionary getConstraints()
        {
            return _constraints.Count > 0 ? _constraints : null;
        }

        public override string ToString()
        {
            return string.Format("{0}", _pattern);
        }
    }

    
}