using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public interface IDependency : DescribesItself
    {
        Type DependencyType { get; }
        void AcceptVisitor(IDependencyVisitor visitor);
        void AssertValid();
    }
}