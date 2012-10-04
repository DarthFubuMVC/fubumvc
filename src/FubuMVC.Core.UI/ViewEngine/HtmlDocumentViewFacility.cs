using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.ViewEngine
{
    public class HtmlDocumentViewFacility : IViewFacility
    {
        private readonly TypePool _types;

        public HtmlDocumentViewFacility(TypePool types)
        {
            _types = types;
        }

        public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
        {
            return _types.TypesMatching(t => t.IsConcrete() && t.Closes(typeof (FubuHtmlDocument<>)))
                .Select(t => new HtmlDocumentViewToken(t));
        }
    }
}