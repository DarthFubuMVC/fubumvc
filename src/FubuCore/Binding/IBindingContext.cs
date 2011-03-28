using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Binding;

namespace FubuCore.Binding
{
    public interface IBindingContext
    {
        IList<ConvertProblem> Problems { get; }

        IBindingContext PrefixWith(string prefix);
        void ForProperty(PropertyInfo property, Action<IPropertyContext> action);

        void ForObject(object @object, Action action);

        void BindChild(PropertyInfo property, Type childType, string prefix);
        void BindChild(PropertyInfo property);
        object BindObject(string prefix, Type childType);

        T Service<T>();
        object Object { get; }
    }

    public interface IPropertyContext
    {
        object PropertyValue { get; }

        PropertyInfo Property { get; }
        object Object { get; }
        T Service<T>();
    }
}