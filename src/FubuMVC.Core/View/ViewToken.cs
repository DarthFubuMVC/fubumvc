using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.View
{
    /// <summary>
    ///   A handle to a view (e.g. a Webforms or Spark View)
    /// </summary>
    public abstract class ViewToken
    {
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public abstract ObjectDef BuildViewFactoryDef();

        public BehaviorNode ToBehavioralNode()
        {
            throw new NotSupportedException();
        }
    }
}