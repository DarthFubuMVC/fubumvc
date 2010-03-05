using System;
using FubuCore.Binding;

namespace FubuCore.Binding
{
    public interface IObjectResolver
    {
        BindResult BindModel(Type type, IRequestData data);
        BindResult BindModel(Type type, IBindingContext context);
    }
}