using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public interface IViewFacility
    {
        IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph);
        BehaviorNode CreateViewNode(Type type);
    }
}