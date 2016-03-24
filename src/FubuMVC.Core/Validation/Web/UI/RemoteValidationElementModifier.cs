using System.Linq;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation.Web.Remote;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class RemoteValidationElementModifier : InputElementModifier
    {
        protected override void modify(ElementRequest request)
        {
            var graph = request.Get<RemoteRuleGraph>();
            var rules = graph.RulesFor(request.Accessor);
            var data = new RemoteValidationDef
            {
                rules = rules.Select(x => x.ToHash()).ToArray(),
                url = request.Get<IUrlRegistry>().RemoteRule()
            };

            if (!data.rules.Any())
            {
                return;
            }

            request.CurrentTag.Data("remote-rule", data);
        }
    }

    public class RemoteValidationDef
    {
        public string url { get; set; }
        public string[] rules { get; set; }
    }
}