using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class EndpointReport
    {
        public const string N_A = "N/A";
        private readonly BehaviorChain _chain;


        public static EndpointReport ForChain(BehaviorChain chain)
        {
            return new EndpointReport(chain);
        }

        public EndpointReport(BehaviorChain chain)
        {
            _chain = chain;
        }

        public IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                {"title", Title},
                {"constraints", Constraints},
                {"actions", Action.ToArray()},
                {"route", Route},
                {"hash", _chain.GetHashCode()},
                {"resource", ResourceType.ToDictionary()},
                {"input", InputModel.ToDictionary()},
                {"origin", Origin},
                {"accepts", Accepts},
                {"authorization", Authorization},
                {"content-type", ContentType},
                {"output", Output.ToArray()},
                {"category", UrlCategory},
                {"wrappers", Wrappers.ToArray()},
                {"tags", _chain.Tags.ToArray()}
            };


            return dict;
        }

        public string Route
        {
            get
            {
                var route = _chain is RoutedChain ? _chain.As<RoutedChain>().GetRoutePattern() : N_A;
                if (route.IsEmpty())
                {
                    route = "(home)";
                }

                return route;
            }
        }

        public Type ResourceType
        {
            get
            {
                // TODO -- FubuContinuation does not count!
                return _chain.ResourceType();
            }
        }

        public Type InputModel
        {
            get { return _chain.InputType(); }
        }

        public IEnumerable<string> Action
        {
            get { return _chain.Calls.Select(x => x.Description); }
        }

        public string Constraints
        {
            get
            {
                var routedChain = _chain as RoutedChain;


                if (routedChain == null) return N_A;

                if (!routedChain.Route.AllowedHttpMethods.Any()) return "Any";

                return routedChain.Route.AllowedHttpMethods.OrderBy(x => x).Join(", ");
            }
        }

        public string Title
        {
            get { return _chain.Title(); }
        }


        public IEnumerable<string> Output
        {
            get
            {
                foreach (var node in _chain.Where(x => x.Category == BehaviorCategory.Output))
                {
                    if (node is OutputNode)
                    {
                        foreach (var writer in node.As<OutputNode>().Media())
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

        public IEnumerable<string> ContentType
        {
            get
            {
                // TODO -- what about other types of nodes that can write?
                // IHaveContentTypes ?????
                var outputNode = _chain.OfType<OutputNode>().FirstOrDefault();
                if (outputNode == null) return Enumerable.Empty<string>();

                return outputNode.Media().SelectMany(x => x.Mimetypes).Distinct();
            }
        }

        public IEnumerable<string> Accepts
        {
            get
            {
                // TODO -- what about other types of nodes that can write?
                // IHaveContentTypes ?????
                var inputNode = _chain.OfType<InputNode>().FirstOrDefault();
                if (inputNode == null) return new[] {MimeType.HttpFormMimetype};

                return inputNode.Readers().SelectMany(x => x.Mimetypes).Distinct();
            }
        }

        public IEnumerable<string> Authorization
        {
            get
            {
                var authorization = _chain.OfType<AuthorizationNode>().FirstOrDefault();
                if (authorization == null || !authorization.Policies.Any()) yield break;

                foreach (var policy in authorization.Policies)
                {
                    yield return Description.For(policy).Title;
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

        public string Origin
        {
            get { return _chain.Origin; }
        }

        public string UrlCategory
        {
            get
            {
                var routed = _chain as RoutedChain;

                return routed == null ? string.Empty : routed.UrlCategory.Category;
            }
        }
    }
}