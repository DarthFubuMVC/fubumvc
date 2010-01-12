using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        Type ViewModelType { get; }
        string Namespace { get; }
        string Name { get; }
        BehaviorNode ToBehavioralNode();
    }
}