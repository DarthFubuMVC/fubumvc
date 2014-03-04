using System;
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

        IRenderableView GetView();
        IRenderableView GetPartialView();

        string ProfileName { get; set; }
    }
}