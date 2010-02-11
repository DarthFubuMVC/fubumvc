using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Models
{
    public interface IBindingContext
    {
        T Service<T>();
        IBindingContext PrefixWith(string prefix);
        IList<ConvertProblem> Problems { get; }
        object PropertyValue { get; }

        PropertyInfo Property { get; }
        void ForProperty(PropertyInfo property, Action action);
        void LogProblem(Exception ex);
        void LogProblem(string exceptionText);

        object Object { get; }
        void StartObject(object @object);
        void FinishObject();
    }


}