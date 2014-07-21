using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Nodes
{
    public class RoutedChain : BehaviorChain
    {
        private readonly IList<IRouteDefinition> _additionalRoutes = new List<IRouteDefinition>();

        /// <summary>
        ///   Creates a new BehaviorChain for an action method
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "expression"></param>
        /// <returns></returns>
        public static RoutedChain For<T>(Expression<Action<T>> expression, string url)
        {
            var call = ActionCall.For(expression);
            var chain = new RoutedChain(new RouteDefinition(url), call.InputType(), call.ResourceType());
            chain.AddToEnd(call);

            return chain;
        }

        public BehaviorNode Authentication { get; set; }

        public RoutedChain(string pattern) : this(new RouteDefinition(pattern))
        {
            
        }

        public RoutedChain(IRouteDefinition route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            
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

        protected internal override void InsertNodes(ConnegSettings settings)
        {
            base.InsertNodes(settings);
            if (Authentication != null)
            {
                InsertFirst(Authentication);
            }
        }

        /// <summary>
        ///   Models how the Route for this BehaviorChain will be generated
        /// </summary>
        public IRouteDefinition Route { get; protected set; }

        /// <summary>
        /// Collection of additional routes (aliases) that will be generated for this BehaviorChain.
        /// </summary>
        public IEnumerable<IRouteDefinition> AdditionalRoutes
        {
            get { return _additionalRoutes; }
        }



        public int Rank
        {
            get { return IsPartialOnly || Route == null ? 0 : Route.Rank; }
        }

        public string RouteName { get; set; }

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


        public override string Title()
        {
            var url = GetRoutePattern();
            if (url.IsEmpty()) return "(home)";

            if (Route.AllowedHttpMethods.Any())
            {
                return Route.AllowedHttpMethods.Select(x => x.ToUpper()).Join("|") + ": " + url;
            }
            
            return url;
        }
    }
}