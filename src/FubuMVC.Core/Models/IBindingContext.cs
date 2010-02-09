using System;
using System.Reflection;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Models
{
    public interface IBindingContext
    {
        T Service<T>();
        void Value(PropertyInfo property, Action<object> callback);
        IBindingContext PrefixWith(string prefix);
    }


}