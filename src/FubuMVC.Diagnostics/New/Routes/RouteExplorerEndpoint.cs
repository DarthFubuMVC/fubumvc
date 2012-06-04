using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;

namespace FubuMVC.Diagnostics.New.Routes
{


    public class RouteReport
    {
        private readonly BehaviorChain _chain;
        public const string NoConstraints = "N/A";

        public RouteReport(BehaviorChain chain, IUrlRegistry urls)
        {
            _chain = chain;
        }

        public Type ResourceType
        {
            get
            {
                // FubuContinuation does not count!
                return _chain.ResourceType();
            }
        }

        public Type InputModel
        {
            get
            {
                return _chain.InputType();
            }
        }

        // 3
        public IEnumerable<string> Action
        {
            get
            {
                return _chain.Calls.Select(x => x.Description);
            }
        }

        // 4:  Look at HttpConstraintResolver for tests
        public string Constraints
        {
            get
            {
                if (_chain.Route == null) return NoConstraints;

                if (_chain.Route != null && !_chain.Route.AllowedHttpMethods.Any()) return "Any";

                return _chain.Route.AllowedHttpMethods.OrderBy(x => x).Join(", ");
            }
        }

        // 5:  Look at RouteColumn for the tests
        public string Route
        {
            get
            {
                if (_chain.IsPartialOnly)
                {
                    return "(partial)";
                }

                if (_chain.Route == null || _chain.Route.Pattern == null)
                {
                    return "N/A";
                }



                var pattern = _chain.Route.Pattern;
                if (pattern == string.Empty)
                {
                    pattern = "(default)";
                }

                return pattern;
            }
        }

        public IEnumerable<string> Output
        {
            get
            {
                throw new NotImplementedException();

                /*
                 * 1.) if none, say "None"
                 * 2.) if not OutputNode, do a short description
                 * 3.) if OutputNode, return one for each output.  Use (for condition)
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 */
            }
        }

        public IEnumerable<string> Accepts
        {
            get
            {
                throw new NotImplementedException();
                // return array of mimetypes from InputNode, or "N/A"
            }
        }

        public IEnumerable<string> ContentType
        {
            get
            {
                throw new NotImplementedException();
                // return array of mimetypes from OutputNode
            }
        }

        public string Authorization
        {
            get
            {
                throw new NotImplementedException();

                // None
                // Loop thru the Authorization policies. 
                // If Type, type.Name
                // If value, value.ToString()
            }
        }

        public string ChainUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Origin
        {
            get
            {
                return _chain.Origin;
            }
        }

        public string UrlCategory
        {
            get
            {
                return _chain.UrlCategory.Category;
            }
        }
    }



    public class RouteExplorerModel
    {
        
    }

    public class RouteExplorerEndpoint
    {
        public RouteExplorerModel get_routes_new()
        {
            return new RouteExplorerModel();
        } 
    }
}