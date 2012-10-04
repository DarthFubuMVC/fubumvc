using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.ViewEngine
{
    public class HtmlDocumentViewFacility : IViewFacility
    {
        public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
        {
            return graph.Types.TypesMatching(t => t.IsConcrete() && t.Closes(typeof (FubuHtmlDocument<>)))
                .Select(t => new HtmlDocumentViewToken(t));
        }
    }
}