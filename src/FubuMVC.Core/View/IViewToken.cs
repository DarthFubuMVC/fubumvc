using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.View
{
    /// <summary>
    /// A handle to a view (e.g. a Webforms or Spark View)
    /// </summary>
    public interface IViewToken
    {
        [MarkedForTermination("Die.")]
        BehaviorNode ToBehavioralNode();

        // Eliminate?
        Type ViewType { get; }
        Type ViewModel { get; }

        // Convert to method
        string Name { get; }

        // See if can convert to namespace/relativepath
        string Folder { get; }
    }

}