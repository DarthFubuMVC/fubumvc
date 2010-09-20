using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.TextWriting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Diagnostics
{
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
            var ul = new HtmlTag("ul");
            tags.Add(ul);
            availableActions().Each(method =>
            {
                var url = DiagnosticUrlPolicy.RootUrlFor(method);
                ul.Child(getDiagnosticActionLink(method, url));
            });

            var diagnosticAssemblies = _graph.Behaviors
                .Where(isDiagnosticChain)
                .GroupBy(chain => chain.FirstCall().HandlerType.Assembly.GetName().Name)
                .OrderBy(group => group.Key);
            foreach (var assembly in diagnosticAssemblies)
            {
                tags.Add(new HtmlTag("h3").Text(assembly.Key));
                var moreUl = new HtmlTag("ul");
                tags.Add(moreUl);
                foreach (var additionalRoute in assembly)
                {
                    moreUl.Child(getDiagnosticActionLink(additionalRoute.FirstCall().Method, additionalRoute.RoutePattern.ToAbsoluteUrl()));
                }
            }

            return BuildDocument("Home", tags.ToArray());
        }

        private HtmlTag getDiagnosticActionLink(MethodInfo method, string url) {
            var li = new HtmlTag("li");
            li.Child(new LinkTag(method.Name, url));
            var description = method.GetAttribute<DescriptionAttribute>().Description;
            li.Child(new HtmlTag("span").Text(" - " + description));
            return li;
        }

        private static bool isDiagnosticChain(BehaviorChain chain)
        {
            return chain.Calls.Any(call => (call.HandlerType.HasAttribute<DiagnosticsActionAttribute>() || call.Method.HasAttribute<DiagnosticsActionAttribute>()) && call.Method.HasAttribute<DescriptionAttribute>());   
        }

        private IEnumerable<MethodInfo> availableActions()
        {
            return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.HasAttribute<DescriptionAttribute>());
        }

        private HtmlDocument BuildDocument(string title, params HtmlTag[] tags)
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
            var pattern = behaviorChain.RoutePattern;
            if( pattern == string.Empty )
            {
                pattern = "(default)";
            }
            document.Child(new HtmlTag("div").Text("Route: " + pattern));

            var nodeTable = new TableTag();
            nodeTable.AddHeaderRow(header =>
            {
                header.Header("Category");
                header.Header("Description");
                header.Header("Type");
            });
            foreach (var node in behaviorChain)
            {

                var description = node.ToString();
                nodeTable.AddBodyRow(row =>
                {
                    row.Cell().Text(node.Category.ToString());
                    row.Cell().Text(description);
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

            content.AddChildren(new[]{
                document, 
                new HtmlTag("h3").Text("Nodes:"),
                nodeTable,
                new HtmlTag("h3").Text("Log:"),
                logDiv});

            return BuildDocument(title, content);
        }

        [Description("show all behavior chains")]
        public HtmlDocument Chains()
        {
            var table = writeTable(x => x.RoutePattern, chains, routes, actions);

            return BuildDocument("Registered Behavior Chains", table);
        } 


        [Description("show all registered routes with their related actions and output")]
        public HtmlDocument Routes()
        {
            var table = writeTable(x => x.Route != null, x => x.RoutePattern, routes, actions, outputs, chains);

            return BuildDocument("Registered Routes", table);
        } 

        public string PrintRoutes()
        {
            return writeTextTable(x => x.RoutePattern, routes, actions, outputs);
        }

        [Description("show all available actions")]
        public HtmlDocument Actions()
        {
            var table = writeTable(x => x.Calls.Any(), x => x.FirstCallDescription, actions, routes, outputs);

            return BuildDocument("Registered Actions", table);
        }

        [Description("show all behavior chains without any authorization rules")]
        public HtmlDocument RoutesWithoutAuthorization()
        {
            var table = writeTable(x => !x.Authorization.HasRules(), x => x.RoutePattern, routes, actions);
            return BuildDocument("Routes without any Authorization Rules", table);
        }

        public string PrintActions()
        {
            return writeTextTable(x => x.FirstCallDescription, actions, routes, outputs);
        }

        [Description("show all input models that are tied to actions")]
        public HtmlDocument Inputs()
        {
            var table = writeTable(x => x.HasInput(), x => x.InputTypeName, inputModels, actions);

            return BuildDocument("Registered Input Types", table);
        }

        [Description("show all authorization rules by actions")]
        public HtmlDocument AuthorizationRulesByActions()
        {
            var table = writeTable(x => x.FirstCall().Description, actions, authorization);
            return BuildDocument("Registered Authorization Rules", table);
        }

        [Description("show all authorization rules by routes")]
        public HtmlDocument AuthorizationRulesByRoutes()
        {
            var table = writeTable(x => x.RoutePattern, routes, authorization);
            return BuildDocument("Registered Authorization Rules", table);
        }

        [Description("list of all the actions allowed by each role")]
        public HtmlDocument ActionsByRole()
        {
            var list = AuthorizationWriter.BuildListOfRoles(_graph, (chain, tag) =>
            {
                tag.Text(chain.Calls.Select(x => x.Description).Join(", "));
            });

            return BuildDocument("Actions by Role", list);
        }

        [Description("list of all the routes allowed by each role")]
        public HtmlDocument RoutesByRole()
        {
            var list = AuthorizationWriter.BuildListOfRoles(_graph, (chain, tag) =>
            {
                tag.Text(chain.RoutePattern);
            });

            return BuildDocument("Routes by Role", list);
        }

        private IColumn routes
        {
            get
            {
                return new RouteColumn();
            }
        }

        private IColumn chains
        {
            get
            {
                return new ChainColumn();
            }
        }

        private IColumn actions
        {
            get
            {
                return new ActionColumn();
            }
        }
        
        private IColumn outputs
        {
            get
            {
                return new OutputColumn();
            }
        }

        private IColumn inputModels
        {
            get
            {
                return new InputModelColumn();
            }
        }

        private IColumn authorization
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

        private HtmlTag writeTable(Func<BehaviorChain, bool> filter, Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            IEnumerable<BehaviorChain> filteredChains = _graph.Behaviors.Where(filter).OrderBy(ordering);

            return WriteBehaviorChainTable(filteredChains, columns);
        }

        private HtmlTag writeTable(Func<BehaviorChain, string> ordering, params IColumn[] columns)
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