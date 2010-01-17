using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;

namespace FubuMVC.Core.Registration.Routes
{
    public interface IRouteDefinition
    {
        string Pattern { get; }
        string Category { get; set; }
        IEnumerable<KeyValuePair<string, object>> Constraints { get; }
        string CreateUrl(object input);
        Route ToRoute();
        void Append(string patternPart);
        void AddRouteInput(RouteInput input, bool b);
        void RemoveLastPatternPart();
        void AddQueryInput(PropertyInfo property);
        void AddRouteConstraint(string inputName, IRouteConstraint constraint);
        void Prepend(string prefix);
    }
}