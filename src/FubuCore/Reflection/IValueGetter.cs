using System;

namespace FubuCore.Reflection
{
    public interface IValueGetter
    {
        object GetValue(object target);
        string Name { get; }
        Type DeclaringType { get; }
    }
}