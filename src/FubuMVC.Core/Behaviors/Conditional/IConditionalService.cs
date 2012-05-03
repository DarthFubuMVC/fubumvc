using System;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public interface IConditionalService
    {
        bool IsTrue(Type type);
    }
}