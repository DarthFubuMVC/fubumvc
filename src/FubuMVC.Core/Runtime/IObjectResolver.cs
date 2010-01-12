using System;
using FubuMVC.Core.Models;

namespace FubuMVC.Core.Runtime
{
    public interface IObjectResolver
    {
        BindResult BindModel(Type type, IRequestData data);
    }
}