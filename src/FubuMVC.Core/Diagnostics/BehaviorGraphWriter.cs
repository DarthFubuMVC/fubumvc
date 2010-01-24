using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.TextWriting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
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
            const string title = "FubuMVC: Diagnostics";

            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text(title);
            var ul = mainDiv.Add("ul");
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

            return BuildDocument(title, mainDiv);
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
            var title = "FubuMVC: Chain " + chainRequest.Id;

            var behaviorChain = _graph.Behaviors.FirstOrDefault(chain => chain.UniqueId == chainRequest.Id);
            if (behaviorChain == null)
            {
                return BuildDocument("Unknown chain", new HtmlTag("span").Text("No behavior chain registered with ID: " + chainRequest.Id));
            }
            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Modify(t =>
            {
                t.Add("span").Text("FubuMVC: Chain ");
                t.Add("span").AddClass("chainId").Text(behaviorChain.UniqueId.ToString());
            });
            var content = mainDiv.Add("div").AddClass("main-content");

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
            behaviorChain.Each(node => nodeTable.AddBodyRow(row =>
            {
                row.Cell().Text(node.Category.ToString());
                row.Cell().Text(node.ToString());
                row.Cell().Text(node.GetType().FullName);
            }));


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

            return BuildDocument(title, mainDiv);
                
        }

        [Description("show all behavior chains")]
        public HtmlDocument Chains()
        {
            const string title = "FubuMVC: Registered Behavior Chains";
            
            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text(title);

            var table = writeTable(x => x.RoutePattern, chains, routes, actions);

            mainDiv.AddChildren(table);
            return BuildDocument(title, mainDiv);
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