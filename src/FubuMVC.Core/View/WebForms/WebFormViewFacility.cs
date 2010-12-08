using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View.WebForms
{
    public class WebFormViewFacility : IViewFacility
    {
        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return types.TypesMatching(IsWebFormView).Select(x => new WebFormViewToken(x) as IViewToken);
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            if (IsWebFormControl(type) || IsWebFormView(type))
            {
                return new WebFormView(type);
            }

            return null;
        }

        public static bool IsWebFormView(Type type)
        {
            return type.CanBeCastTo<Page>() && type.CanBeCastTo<IFubuView>();
        }

        public static bool IsWebFormControl(Type type)
        {
            return type.CanBeCastTo<UserControl>();
        }
    }
}