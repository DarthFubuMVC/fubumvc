using System;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public interface IConditionalService
    {
        bool IsTrue(Type type);
    }
}