using System.Collections.Generic;
using System.Data;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Http
{
    [ApplicationLevel]
    public class ConnegSettings : DescribesItself
    {
        public readonly ConnegRules Rules = new ConnegRules();

        public ConnegSettings()
        {
            Rules.AddToEnd<SymmetricJson>();
            Rules.AddToEnd<AsymmetricJson>();
            Rules.AddToEnd<AjaxContinuations>();
            Rules.AddToEnd<StringOutput>();
            Rules.AddToEnd<HtmlTagsRule>();
            Rules.AddToEnd<CustomReadersAndWriters>();
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

        public ConnegGraph Graph { get; set; }

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
            new JsonSerializer(),
            new XmlFormatter()
        }; 

        public readonly IList<IMimetypeCorrection> Corrections = new List<IMimetypeCorrection>();

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