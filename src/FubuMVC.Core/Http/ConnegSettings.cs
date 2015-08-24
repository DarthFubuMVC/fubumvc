using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Http
{
    public class ConnegSettings : DescribesItself
    {
        public readonly ConnegRules Rules = new ConnegRules();

        public ConnegSettings()
        {
            Rules.AddToEnd<AjaxContinuations>();
            Rules.AddToEnd<StringOutput>();
            Rules.AddToEnd<HtmlTagsRule>();
            Rules.AddToEnd<CustomReadersAndWriters>();
            Rules.AddToEnd<ViewAttachment>();
            Rules.AddToEnd<DefaultReadersAndWriters>();
        }

        void DescribesItself.Describe(Description _)
        {
            _.Title = "Content Negotiation Rules and Configuration";

            if (Formatters.Any())
            {
                _.AddList("Formatters", Formatters);
            }

            _.AddList("Rules", Rules);
            _.AddList("Conneg Querystring Rules", QuerystringParameters);

            if (Corrections.Any())
            {
                _.AddList("Mimetype Corrections", Corrections);
            }
        }

        public void ReadConnegGraph(BehaviorGraph graph)
        {
            _graph = ConnegGraph.Build(graph);
        }

        public void StoreViews(Task<ViewBag> views)
        {
            _views = views;
        }

        public ConnegGraph Graph
        {
            get
            {
                // This makes UT's run cleanly
                if (_graph == null) return null;

                _graph.Wait(5.Seconds());
                return _graph.Result;
            }
        }

        public ViewBag Views
        {
            get
            {
                if (_views == null) return new ViewBag(Enumerable.Empty<IViewToken>());

                _views.Wait(5.Seconds());
                return _views.Result;
            }
        }

        public void ApplyRules(InputNode node)
        {
            Rules.Top.ApplyInputs(node, node.ParentChain() ?? new BehaviorChain(), this);
        }

        public void ApplyRules(OutputNode node)
        {
            Rules.Top.ApplyOutputs(node, node.ParentChain() ?? new BehaviorChain(), this);
        }

        public readonly IList<ConnegQuerystring> QuerystringParameters =
            new List<ConnegQuerystring>
            {
                new ConnegQuerystring("Format", "JSON", MimeType.Json),
                new ConnegQuerystring("Format", "XML", MimeType.Xml)
            };

        public readonly IList<IFormatter> Formatters = new List<IFormatter>
        {
            new NewtonsoftJsonFormatter(),
            new XmlFormatter()
        };

        public readonly IList<IMimetypeCorrection> Corrections = new List<IMimetypeCorrection>();
        private Task<ConnegGraph> _graph;
        private Task<ViewBag> _views;

        public void InterpretQuerystring(CurrentMimeType mimeType, IHttpRequest request)
        {
            var corrected = QuerystringParameters.FirstValue(x => x.Determine(request.QueryString));
            if (corrected.IsNotEmpty())
            {
                mimeType.AcceptTypes = new MimeTypeList(corrected);
            }
        }

        public IFormatter FormatterFor(MimeType mimeType)
        {
            return FormatterFor(mimeType.Value);
        }

        private IFormatter FormatterFor(string mimeType)
        {
            return Formatters.FirstOrDefault(x => x.MatchingMimetypes.Contains(mimeType.ToLowerInvariant()));
        }

        public void AddFormatter(IFormatter formatter)
        {
            Formatters.Insert(0, formatter);
        }
    }
}