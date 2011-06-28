using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public class WebFormViewFacility : IViewFacility
    {
        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return types
                    .TypesMatching(IsWebFormView)
                    .Select(x => new WebFormViewToken(x) as IViewToken);
        }

        public static bool IsWebFormView(Type type)
        {
            return type.CanBeCastTo<Page>() && type.CanBeCastTo<IFubuPage>();
        }

        public static bool IsWebFormControl(Type type)
        {
            return type.CanBeCastTo<UserControl>();
        }
    }
}