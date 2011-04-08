using System;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public interface IDependency
    {
        Type DependencyType { get; }
        void AcceptVisitor(IDependencyVisitor visitor);
        void AssertValid();
    }
}