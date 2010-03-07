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

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorGraphWriter
    {
        private const string sourceControlUrlBase = "http://github.com/DarthFubuMVC/fubumvc/";
        private const string sourceControlUrlFormat = sourceControlUrlBase + "commit/{0}";
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public BehaviorGraphWriter(BehaviorGraph graph, IUrlRegistry urls)
        {
            _graph = graph;
            _urls = urls;
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
                    var description = method.GetAttribute<DescriptionAttribute>().Description;
                    x.Child(new HtmlTag("span").Text(" - " + description));
                });
            });

            return BuildDocument("Home", ul);
        }

        private IEnumerable<MethodInfo> availableActions()
        {
            return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.HasAttribute<DescriptionAttribute>());
        }

        private HtmlDocument BuildDocument(string title, params HtmlTag[] tags)
        {
            return BuildDocument(_urls, title, tags);
        }

        public static HtmlDocument BuildDocument(IUrlRegistry urls, string title, params HtmlTag[] tags)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(BehaviorGraphWriter), "diagnostics.css");
            var reader = new StreamReader(stream);
            var css = reader.ReadToEnd();

            var realTitle = "FubuMVC: " + title;

            var document = new HtmlDocument();
            document.Title = realTitle;

            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text("FubuMVC Diagnostics").Child(buildVersionTag());
            var navBar = mainDiv.Add("div").AddClass("homelink");
            navBar.AddChildren(new LinkTag("Home", urls.UrlFor<BehaviorGraphWriter>(w => w.Index())));
            navBar.Add("span").Text(" > " + title);
            document.Add(mainDiv);

            mainDiv.AddChildren(tags);

            document.AddStyle(css);

            return document;
        }

        private static HtmlTag buildVersionTag()
        {
            var fubuAssembly = typeof(BehaviorGraphWriter).Assembly;
            var attribute = fubuAssembly.GetAttribute<AssemblyDescriptionAttribute>();
            var version = (attribute == null) ? null : attribute.Description;
            var commitAttribute = fubuAssembly.GetAttribute<AssemblyTrademarkAttribute>();
            var commit = commitAttribute == null ? null : commitAttribute.Trademark;
            var versionUrl = commit.IsNotEmpty() ? sourceControlUrlFormat.ToFormat(commit) : sourceControlUrlBase;
            return new HtmlTag("span").Id("version-display").Text("version: ").Child(new LinkTag(version, versionUrl).Attr("title", commit));
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