using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View
{
    /// <summary>
    ///   A handle to a view (e.g. a Webforms or Spark View)
    /// </summary>
    public interface IViewToken
    {
        // Eliminate?
        Type ViewType { get; }
        Type ViewModel { get; }

        // Convert to method
        string Name();

        string Namespace { get; }

        // TODO -- going to get rid of this.
        [MarkedForTermination]
        ObjectDef ToViewFactoryObjectDef();

        IRenderableView GetView();
        IRenderableView GetPartialView();

        string ProfileName { get; set; }
    }
}