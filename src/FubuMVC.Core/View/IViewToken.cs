using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    /// <summary>
    /// A handle to a view (e.g. a Webforms or Spark View)
    /// </summary>
    public interface IViewToken
    {
        BehaviorNode ToBehavioralNode();
        Type ViewType { get; }
        Type ViewModelType { get; }
        string Name { get; }
        string Folder { get; }
    }
}