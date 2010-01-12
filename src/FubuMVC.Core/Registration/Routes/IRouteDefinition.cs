using System.Reflection;
using System.Web.Routing;

namespace FubuMVC.Core.Registration.Routes
{
    public interface IRouteDefinition
    {
        string Pattern { get; }
        string Category { get; set; }
        string CreateUrl(object input);
        Route ToRoute();
        void Append(string patternPart);
        void AddRouteInput(RouteInput input, bool b);
        void RemoveLastPatternPart();
        void AddQueryInput(PropertyInfo property);

        void Prepend(string prefix);
    }
}