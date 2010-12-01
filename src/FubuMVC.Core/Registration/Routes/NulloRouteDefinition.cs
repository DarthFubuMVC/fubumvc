using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Routes
{
    public class NulloRouteDefinition : IRouteDefinition
    {
        public NulloRouteDefinition()
            : this("(unroutable)")
        {

        }

        public NulloRouteDefinition(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; private set; }

        public string Category { get; set; }

        public Type InputType
        {
            get { return null; }
        }

        public int Rank
        {
            get { return 0; }
        }

        public IEnumerable<KeyValuePair<string, object>> Constraints
        {
            get { return new KeyValuePair<string, object>[0]; }
        }

        public string CreateUrl(object input)
        {
            return Pattern;
        }

        public void RootUrlAt(string baseUrl)
        {

        }

        public Route ToRoute()
        {
            return new IgnoredRoute(Pattern);
        }

        public void Append(string patternPart)
        {

        }

        public void AddRouteInput(RouteInput input, bool appendToUrl)
        {

        }

        public void RemoveLastPatternPart()
        {

        }

        public void AddQueryInput(PropertyInfo property)
        {

        }

        public void AddRouteConstraint(string inputName, IRouteConstraint constraint)
        {
        }

        public void Prepend(string prefix)
        {
        }
    }
}