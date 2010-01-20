using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        BehaviorNode ToBehavioralNode();
        Type ViewType { get; }
        Type ViewModelType { get; }
        string Name { get; }
        string Folder { get; }
    }
}