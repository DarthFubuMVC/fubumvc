using System;
using FubuMVC.Core.Models;

namespace FubuMVC.Core.Runtime
{
    public interface IModelBinderCache
    {
        IModelBinder BinderFor(Type modelType);
    }
}