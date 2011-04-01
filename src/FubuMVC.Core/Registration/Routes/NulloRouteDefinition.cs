using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Routes
{
    [Obsolete]
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

        public string CreateTemplate(object input, Func<object, object>[] hash)
        {
            throw new NotImplementedException();
        }

        public IRouteInput Input
        {
            get { return null; }
        }

        public string CreateUrlFromInput(object input)
        {
            throw new NotImplementedException();
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

        public void RemoveLastPatternPart()
        {

        }

        public void AddRouteConstraint(string inputName, IRouteConstraint constraint)
        {
        }

        public void Prepend(string prefix)
        {
        }

        private static IRouteDefinition _flyweight = new NulloRouteDefinition();
        public static IRouteDefinition Flyweight()
        {
            return _flyweight;
        }
    }
}