using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public interface IModelBinder
    {
        bool Matches(Type type);
        BindResult Bind(Type type, object instance, IRequestData data);
        BindResult Bind(Type type, IRequestData data);
    }
}