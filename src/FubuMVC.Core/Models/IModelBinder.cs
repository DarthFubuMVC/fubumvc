using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public interface IModelBinder
    {
        bool Matches(Type type);
        BindResult Bind(Type type, object instance, IBindingContext context);
        BindResult Bind(Type type, IBindingContext context);
    }
}