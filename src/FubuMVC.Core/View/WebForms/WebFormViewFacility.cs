using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.WebForms
{
    public class WebFormViewFacility : IViewFacility
    {
        public IEnumerable<IDiscoveredViewToken> FindViews(TypePool types)
        {
            return types.TypesMatching(IsWebFormView).Select(x => new WebFormViewToken(x) as IDiscoveredViewToken);
        }

        public static bool IsWebFormView(Type type)
        {
            return type.CanBeCastTo<Page>() && type.CanBeCastTo<IFubuView>();
        }
    }
}