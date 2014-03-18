using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Nodes
{
    public class RoutedChain : BehaviorChain
    {
        private readonly IList<IRouteDefinition> _additionalRoutes = new List<IRouteDefinition>();

        public RoutedChain(string pattern) : this(new RouteDefinition(pattern))
        {
            
        }

        public RoutedChain(IRouteDefinition route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            UrlCategory = new UrlCategory();
            Route = route;
        }

        public RoutedChain(IRouteDefinition route, Type inputType, Type resourceType) : this(route)
        {
            if (inputType != null)
            {
                route.ApplyInputType(inputType);
            }

            if (resourceType != null)
            {
                ResourceType(resourceType);
            }
        }

        /// <summary>
        ///   Models how the Route for this BehaviorChain will be generated
        /// </summary>
        public IRouteDefinition Route { get; private set; }

        /// <summary>
        /// Collection of additional routes (aliases) that will be generated for this BehaviorChain.
        /// </summary>
        public IEnumerable<IRouteDefinition> AdditionalRoutes
        {
            get { return _additionalRoutes; }
        }

        /// <summary>
        ///   Categorizes this BehaviorChain for the IUrlRegistry and 
        ///   IEndpointService UrlFor(***, category) methods
        /// </summary>
        public UrlCategory UrlCategory { get; private set; }

        public int Rank
        {
            get { return IsPartialOnly || Route == null ? 0 : Route.Rank; }
        }

        /// <summary>
        /// Adds the specified route as an additional route for this BehaviorChain to respond to.
        /// </summary>
        /// <param name="route"></param>
        public void AddRouteAlias(IRouteDefinition route)
        {
            _additionalRoutes.Fill(route);
        }

        public string GetRoutePattern()
        {
            if (Route == null) return null;

            return Route.Pattern;
        }

        /// <summary>
        ///   Does this chain match by either UrlCategory or by Http method?
        /// </summary>
        /// <param name = "categoryOrHttpMethod"></param>
        /// <returns></returns>
        public override bool MatchesCategoryOrHttpMethod(string categoryOrHttpMethod)
        {
            if (UrlCategory.Category.IsNotEmpty() &&
                UrlCategory.Category.Equals(categoryOrHttpMethod, StringComparison.OrdinalIgnoreCase)) return true;

            if (Route == null) return false;

            return Route.AllowedHttpMethods.Select(x => x.ToUpper()).Contains(categoryOrHttpMethod.ToUpper());
        }

        /// <summary>
        ///   Prepends the prefix to the route definition
        /// </summary>
        /// <param name = "prefix"></param>
        public void PrependToUrl(string prefix)
        {
            if (Route != null)
            {
                Route.Prepend(prefix);
            }
        }

        public override string ToString()
        {
            var description = Route.Pattern;
            if (Route.AllowedHttpMethods.Any())
            {
                description += " (" + Route.AllowedHttpMethods.Join(", ") + ")";
            }

            return description;
        }

        public override string Category
        {
            get { return UrlCategory == null ? null : UrlCategory.Category; }
        }
    }
}