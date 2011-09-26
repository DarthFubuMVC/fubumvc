using System;
using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingPropertyBinderWrapper : IPropertyBinderCache
    {
        private readonly PropertyBinderCache _inner;
        private readonly IDebugReport _report;

        public RecordingPropertyBinderWrapper(PropertyBinderCache inner, IDebugReport report)
        {
            _inner = inner;
            _report = report;
        }

        public IPropertyBinder BinderFor(PropertyInfo property)
        {
            var binder = _inner.BinderFor(property);
            if (binder != null)
            {
                var selection = new PropertyBinderSelection{
                    BinderType = binder.GetType(),
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType
                };

                _report.AddBindingDetail(selection);
            }
            return binder;
        }
    }
}