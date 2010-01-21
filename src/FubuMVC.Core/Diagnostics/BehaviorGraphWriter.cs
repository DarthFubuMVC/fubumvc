using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.TextWriting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorGraphWriter
    {
        private readonly BehaviorGraph _graph;

        public BehaviorGraphWriter(BehaviorGraph graph)
        {
            _graph = graph;
        }

        [UrlPattern("_fubu")]
        public HtmlDocument Index()
        {
            var ul = new HtmlTag("ul");
            availableActions().Each(method =>
            {
                var url = DiagnosticUrlPolicy.RootUrlFor(method);
                ul.Add("li").Modify(x =>
                {
                    x.Child(new LinkTag(method.Name, url));
                    var description = method.GetCustomAttribute<DescriptionAttribute>().Description;
                    x.Child(new HtmlTag("span").Text(" - " + description));
                });
            });

            return BuildDocument("Fubu Diagnostics", ul);
        }

        private IEnumerable<MethodInfo> availableActions()
        {
            return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.HasCustomAttribute<DescriptionAttribute>());
        }

        public static HtmlDocument BuildDocument(string title, params HtmlTag[] tags)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(BehaviorGraphWriter), "diagnostics.css");
            var reader = new StreamReader(stream);
            var css = reader.ReadToEnd();

            var document = new HtmlDocument();
            document.Title = title;
            tags.Each(x => document.Add(x));

            document.AddStyle(css);

            return document;
        }

        public HtmlDocument Chain(ChainRequest chainRequest)
        {
            var behaviorChain = _graph.Behaviors.FirstOrDefault(chain => chain.UniqueId == chainRequest.Id);
            if (behaviorChain == null)
            {
                return BuildDocument("Unknown chain", new HtmlTag("span").Text("No behavior chain registered with ID: " + chainRequest.Id));
            }
            var heading = new HtmlTag("h1").Modify(t =>
            {
                t.Add("span").Text("Chain ");
                t.Add("span").AddClass("chainId").Text(behaviorChain.UniqueId.ToString());
            });
            var document = new HtmlTag("div");
            document.Child(new HtmlTag("div").Text("Route: " + behaviorChain.RoutePattern));

            var nodeTable = new TableTag();
            nodeTable.AddHeaderRow(header =>
            {
                header.Header("Category");
                header.Header("Description");
                header.Header("Type");
            });
            behaviorChain.Each(node => nodeTable.AddBodyRow(row =>
            {
                row.Cell().Text(node.Category.ToString());
                row.Cell().Text(node.ToString());
                row.Cell().Text(node.GetType().FullName);
            }));
            return BuildDocument("Chain " + chainRequest.Id, heading, document, new HtmlTag("h2").Text("Nodes:"), nodeTable);
        }

        [Description("show all behavior chains")]
        public HtmlDocument Chains()
        {
            var table = writeTable(x => x.RoutePattern, chains, routes, actions);

            return BuildDocument("Registered Fubu Behavior Chains", table);
        } 


        [Description("show all registered routes with their related actions and output")]
        public HtmlDocument Routes()
        {
            var table = writeTable(x => x.Route != null, x => x.RoutePattern, routes, actions, outputs, chains);

            return BuildDocument("Registered Fubu Routes", table);
        } 

        public string PrintRoutes()
        {
            return writeTextTable(x => x.RoutePattern, routes, actions, outputs);
        }

        [Description("show all available actions")]
        public HtmlDocument Actions()
        {
            var table = writeTable(x => x.Calls.Any(), x => x.FirstCallDescription, actions, routes, outputs);
            return BuildDocument("Registered Fubu Actions", table);
        }

        public string PrintActions()
        {
            return writeTextTable(x => x.FirstCallDescription, actions, routes, outputs);
        }

        [Description("show all input models that are tied to actions")]
        public HtmlDocument Inputs()
        {
            var table = writeTable(x => x.HasInput(), x => x.InputTypeName, inputModels, actions);
            return BuildDocument("Registered Fubu Input Types", table);
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
            IEnumerable<BehaviorChain> chains = _graph.Behaviors.Where(filter).OrderBy(ordering);

            return writeTable(chains, columns);
        }

        private HtmlTag writeTable(Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            return writeTable(x => true, ordering, columns);
        }

        private HtmlTag writeTable(IEnumerable<BehaviorChain> chains, params IColumn[] columns)
        {
            var table = new TableTag();
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
                        col.WriteBody(chain, row.Cell());
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