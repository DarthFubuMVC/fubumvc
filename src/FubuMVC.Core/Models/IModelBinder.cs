using System;

namespace FubuMVC.Core.Models
{
    public interface IModelBinder
    {
        bool Matches(Type type);
        void Bind(Type type, object instance, IBindingContext context);
        object Bind(Type type, IBindingContext context);
    }
}