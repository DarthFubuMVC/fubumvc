using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Diagnostics.TextWriting;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    [WannaKill]
    [FubuDiagnostics("Authorization")]
    public class AuthorizationDiagnosticWriter
    {
        private readonly BehaviorGraphWriter _writer;
        private readonly BehaviorGraph _graph;

        public AuthorizationDiagnosticWriter(BehaviorGraphWriter writer, BehaviorGraph graph)
        {
            _writer = writer;
            _graph = graph;
        }

        [FubuDiagnostics("Routes with no authorization rules or policies")]
        public HtmlDocument RoutesWithoutAuthorization()
        {
            var table = _writer.writeTable(x => !x.Authorization.HasRules(), x => x.GetRoutePattern(), _writer.routes, _writer.actions);
            return _writer.BuildDocument("Routes without any Authorization Rules", table);
        }

        [FubuDiagnostics("Authorization rules by action")]
        public HtmlDocument AuthorizationRulesByActions()
        {
            var table = _writer.writeTable(x => x.FirstCall().Description, _writer.actions, _writer.authorization);
            return _writer.BuildDocument("Registered Authorization Rules", table);
        }

        [FubuDiagnostics("Authorization rules by route")]
        public HtmlDocument AuthorizationRulesByRoutes()
        {
            var table = _writer.writeTable(x => x.GetRoutePattern(), _writer.routes, _writer.authorization);
            return _writer.BuildDocument("Registered Authorization Rules", table);
        }

        [FubuDiagnostics("Actions allowed by each role")]
        public HtmlDocument ActionsByRole()
        {
            var list = AuthorizationWriter.BuildListOfRoles(_graph, (chain, tag) =>
            {
                tag.Text(chain.Calls.Select(x => x.Description).Join(", "));
            });

            return _writer.BuildDocument("Actions by Role", list);
        }

        [FubuDiagnostics("Routes allowed by each role")]
        public HtmlDocument RoutesByRole()
        {
            var list = AuthorizationWriter.BuildListOfRoles(_graph, (chain, tag) =>
            {
                tag.Text(chain.GetRoutePattern());
            });

            return _writer.BuildDocument("Routes by Role", list);
        }
    }

    public static class BehaviorGraphExtensions
    {
        public static string FirstCallDescription(this BehaviorChain chain)
        {
            var call = chain.FirstCall();
            return call == null ? string.Empty : call.Description;
        }

        public static string GetRoutePattern(this BehaviorChain chain)
        {
            return chain.Route == null ? string.Empty : chain.Route.Pattern;
        }

        public static string GetInputTypeName(this BehaviorChain chain)
        {
            var type = chain.InputType();
            return type == null ? string.Empty : type.Name;
        }
    }


    [FubuDiagnostics("Behavior Graph Information")]
    public class BehaviorGraphWriter
    {
        public const string FUBU_INTERNAL_CLASS = "fubu-internal";
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;
        private readonly string _diagnosticsNamespace;

        public BehaviorGraphWriter(BehaviorGraph graph, IUrlRegistry urls, IServiceLocator services)
        {
            _graph = graph;
            _urls = urls;
            _services = services;
            _diagnosticsNamespace = GetType().Namespace;
        }

        [UrlPattern(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT)]
        public HtmlDocument Index()
        {
            var tags = new List<HtmlTag>();



            var groups = _graph
                .Actions()
                .Where(x => x.HandlerType.HasAttribute<FubuDiagnosticsAttribute>() && (!x.HasInput || x.HandlerType.GetAttribute<FubuDiagnosticsAttribute>().ShownInIndex) && x.Method.Name != "Index")
                .GroupBy(x => x.HandlerType.GetAttribute<FubuDiagnosticsAttribute>().Description)
                .OrderBy(x => x.Key);

            groups.Each(group =>
            {
                tags.Add(new HtmlTag("h3").Text(group.Key));
                var ul = new HtmlTag("ul");
                tags.Add(ul);

                
                group.Each<ActionCall>(action =>
                {
                    string text = action.Method.Name;
                    action.Method.ForAttribute<FubuDiagnosticsAttribute>(att => text = att.Description);

                    ul.Add("li/a").Text(text)
                        .Attr("href", action.ParentChain().GetRoutePattern());
                });
            });



            return BuildDocument("Home", tags.ToArray());
        }
        
        internal HtmlDocument BuildDocument(string title, params HtmlTag[] tags)
        {
            return DiagnosticHtml.BuildDocument(_urls, title, tags);
        }

        public HtmlDocument Chain(ChainRequest chainRequest)
        {
            var title = "Chain " + chainRequest.Id;

            var behaviorChain = _graph.Behaviors.FirstOrDefault(chain => chain.UniqueId == chainRequest.Id);
            if (behaviorChain == null)
            {
                return BuildDocument("Unknown chain", new HtmlTag("span").Text("No behavior chain registered with ID: " + chainRequest.Id));
            }
           
            var content = new HtmlTag("div").AddClass("main-content");

            var document = new HtmlTag("div");
            var pattern = behaviorChain.GetRoutePattern();
            if( pattern == string.Empty )
            {
                pattern = "(default)";
            }
            document.Append(new HtmlTag("div").Text("Route: " + pattern));

            var nodeTable = new TableTag();
            nodeTable.AddHeaderRow(header =>
            {
                header.Header("Category");
                header.Header("Description");
                header.Header("Type");
            });
            foreach (var node in behaviorChain)
            {

                var description = node.ToString().HtmlEncode().ConvertCRLFToBreaks();
                nodeTable.AddBodyRow(row =>
                {
                    row.Cell().Text(node.Category.ToString());
                    row.Cell().Encoded(false).Text(description);
                    row.Cell().Text(node.GetType().FullName);
                    if (description.Contains(_diagnosticsNamespace))
                    {
                        row.AddClass(FUBU_INTERNAL_CLASS);
                    }
               });
            }


            var logDiv = new HtmlTag("div").AddClass("convention-log");
            var ul = logDiv.Add("ul");

            var observer = _graph.Observer;
            behaviorChain.Calls.Each(
                call => observer.GetLog(call).Each(
                            entry => ul.Add("li").Text(entry)));

            content.Append(new[]{
                document, 
                new HtmlTag("h3").Text("Nodes:"),
                nodeTable,
                new HtmlTag("h3").Text("Log:"),
                logDiv});

            return BuildDocument(title, content);
        }

        [FubuDiagnostics("Behavior chains")]
        public HtmlDocument Chains()
        {
            var table = writeTable(x => x.GetRoutePattern(), chains, constraints, routes, actions);

            return BuildDocument("Registered Behavior Chains", table);
        } 


        [FubuDiagnostics("Registered routes with their related actions and output")]
        public HtmlDocument Routes()
        {
            var table = writeTable(x => x.Route != null, x => x.GetRoutePattern(), constraints, routes, actions, outputs, chains);

            return BuildDocument("Registered Routes", table);
        }

        [FubuDiagnostics("Print the available routes")]
        public string PrintRoutes()
        {
            return writeTextTable(x => x.GetRoutePattern(), routes, actions, outputs);
        }

        [FubuDiagnostics("Available actions")]
        public HtmlDocument Actions()
        {
            var table = writeTable(x => x.Calls.Any(), x => x.FirstCallDescription(), actions, routes, outputs);

            return BuildDocument("Registered Actions", table);
        }


        [FubuDiagnostics("Print the available actions")]
        public string PrintActions()
        {
            return writeTextTable(x => x.FirstCallDescription(), actions, routes, outputs);
        }





        internal IColumn routes
        {
            get
            {
                return new RouteColumn();
            }
        }

        internal IColumn chains
        {
            get
            {
                return new ChainColumn();
            }
        }

        internal IColumn actions
        {
            get
            {
                return new ActionColumn();
            }
        }
        
        internal IColumn outputs
        {
            get
            {
                return new OutputColumn();
            }
        }

        internal IColumn inputModels
        {
            get
            {
                return new InputModelColumn();
            }
        }

        internal IColumn constraints
        {
            get
            {
                return new ConstraintColumn();
            }
        }

        internal IColumn authorization
        {
            get
            {
                return new AuthorizationRulesColumn(_services);
            }
        }

        private string writeTextTable(Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            var chains = _graph.Behaviors.OrderBy(ordering);
            
            var writer = new TextReportWriter(columns.Count());
            writer.AddText(columns.Select(x => x.Header()).ToArray());
            writer.AddDivider('=');

            chains.Each(chain =>
            {
                string[] data = columns.Select(x => x.Text(chain)).ToArray();
                writer.AddText(data);
                writer.AddDivider('-');
            });

            return writer.Write();
        }

        internal HtmlTag writeTable(Func<BehaviorChain, bool> filter, Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            IEnumerable<BehaviorChain> filteredChains = _graph.Behaviors.Where(filter).OrderBy(ordering);

            return WriteBehaviorChainTable(filteredChains, columns);
        }

        internal HtmlTag writeTable(Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            return writeTable(x => true, ordering, columns);
        }

        public static HtmlTag WriteBehaviorChainTable(IEnumerable<BehaviorChain> chains, params IColumn[] columns)
        {
            var table = new TableTag();
            table.Attr("cellspacing", "0");
            table.Attr("cellpadding", "0");
            table.AddHeaderRow(row =>
            {
                columns.Each(c => row.Header(c.Header()));
            });

            chains.Each(chain =>
            {
                table.AddBodyRow(row =>
                {
                    columns.Each(col =>
                    {
                        col.WriteBody(chain, row, row.Cell());
                    });
                });
            });

            return table;
        }
    }

    public class ChainRequest
    {
        public Guid Id { get; set; }
    }
}