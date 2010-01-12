using System;
using System.Collections.Generic;
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
            GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.DeclaringType == GetType()).Where(x => x.Name != "Index").Each(method =>
            {
                string url = DiagnosticUrlPolicy.UrlFor(method);
                ul.Add("li").Child(new LinkTag(method.Name, url));
            });

            return BuildDocument("Fubu Diagnostics", ul);
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

        public HtmlDocument RoutesTable()
        {
            var table = writeTable(x => x.RoutePattern, routes, actions, outputs);

            return BuildDocument("Registered Fubu Routes", table);
        } 

        public string Routes()
        {
            return writeTextTable(x => x.RoutePattern, routes, actions, outputs);
        }

        public HtmlDocument ActionsTable()
        {
            var table = writeTable(x => x.FirstCallDescription, actions, routes, outputs);
            return BuildDocument("Registered Fubu Actions", table);
        }

        public string Actions()
        {
            return writeTextTable(x => x.FirstCallDescription, actions, routes, outputs);
        }

        public HtmlDocument InputsTable()
        {
            var table = writeTable(_graph.Behaviors.Where(x => x.HasInput()).OrderBy(x => x.InputTypeName), inputModels, actions);
            return BuildDocument("Registered Fubu Input Types", table);
        }

        public HtmlDocument AllUrls()
        {
            throw new NotImplementedException();
        }


        private IColumn routes
        {
            get
            {
                return new RouteColumn();
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

        private HtmlTag writeTable(Func<BehaviorChain, string> ordering, params IColumn[] columns)
        {
            IEnumerable<BehaviorChain> chains = _graph.Behaviors.OrderBy(ordering);

            return writeTable(chains, columns);
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
}