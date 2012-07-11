using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;
using FubuCore.Reflection;
using FubuMVC.Diagnostics.Chrome;
using FubuMVC.Diagnostics.Features.Chains;
using FubuMVC.SlickGrid;

namespace FubuMVC.Diagnostics.New.Routes
{
    public class RouteQuery
    {
        
    }

    public class RouteSource : IGridDataSource<RouteReport, RouteQuery>
    {
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public RouteSource(BehaviorGraph graph, IUrlRegistry urls)
        {
            _graph = graph;
            _urls = urls;
        }

        public IEnumerable<RouteReport> GetData(RouteQuery query)
        {
            return _graph.Behaviors.Select(x => new RouteReport(x, _urls));
        }
    }

    public class RoutesGrid : GridDefinition<RouteReport>
    {
        public RoutesGrid()
        {
            SourceIs<RouteSource>();

            Column(x => x.Route).Width(width:300);
            Column(x => x.Constraints).Width(width: 100);
            Column(x => x.Action).Formatter(SlickGridFormatter.StringArray);
        }
    }

    public class RouteReport
    {
        private readonly BehaviorChain _chain;
        private readonly string _chainUrl;
        public const string NoConstraints = "N/A";

        public RouteReport(BehaviorChain chain, IUrlRegistry urls)
        {
            _chain = chain;
            _chainUrl = urls.UrlFor(new ChainRequest { Id = chain.UniqueId });
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

        public IEnumerable<string> Action
        {
            get
            {
                return _chain.Calls.Select(x => x.Description);
            }
        }

        public string Constraints
        {
            get
            {
                if (_chain.Route == null) return NoConstraints;

                if (_chain.Route != null && !_chain.Route.AllowedHttpMethods.Any()) return "Any";

                return _chain.Route.AllowedHttpMethods.OrderBy(x => x).Join(", ");
            }
        }

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
                foreach (BehaviorNode node in _chain.Where(x => x.Category == BehaviorCategory.Output))
                {
                    if (node is OutputNode)
                    {
                        foreach (var writer in node.As<OutputNode>().Writers)
                        {
                            yield return writer.ToString();
                        }
                    }
                    else
                    {
                        yield return Description.For(node).Title;
                    }
                }
            }
        }

        public IEnumerable<string> Accepts
        {
            get
            {
                // TODO -- what about other types of nodes that can write?
                // IHaveContentTypes ?????
                var outputNode = _chain.OfType<OutputNode>().FirstOrDefault();
                if (outputNode == null) return Enumerable.Empty<string>();

                return outputNode.Writers.SelectMany(x => x.Mimetypes).Distinct();
            }
        }

        public IEnumerable<string> ContentType
        {
            get
            {
                // TODO -- what about other types of nodes that can write?
                // IHaveContentTypes ?????
                var inputNode = _chain.OfType<InputNode>().FirstOrDefault();
                if (inputNode == null) return Enumerable.Empty<string>();

                return inputNode.Readers.SelectMany(x => x.Mimetypes).Distinct();
            }
        }

        public IEnumerable<string> Authorization
        {
            get
            {
                var authorization = _chain.OfType<AuthorizationNode>().FirstOrDefault();
                if (authorization == null || !authorization.AllRules.Any()) yield break;

                foreach (var objectDef in authorization.AllRules)
                {
                    if (objectDef.Value != null)
                    {
                        yield return Description.For(objectDef.Value).Title;
                    }
                    else
                    {
                        // PUt on description
                        if (objectDef.Type.HasAttribute<TitleAttribute>())
                        {
                            yield return objectDef.Type.GetAttribute<TitleAttribute>().Title;
                        }
                        else
                        {
                            yield return objectDef.Type.Name;
                        }
                    }
                }
            }
        }

        public IEnumerable<string> Wrappers
        {
            get
            {
                // TODO -- might be a pretty type opportunity
                return _chain.OfType<Wrapper>().Select(x => x.BehaviorType.Name);
            }
        }

        public string ChainUrl
        {
            get
            {
                return _chainUrl;
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
        [WrapWith(typeof(ChromeBehavior))]
        public RouteExplorerModel get_routes_new()
        {
            return new RouteExplorerModel();
        } 
    }
}