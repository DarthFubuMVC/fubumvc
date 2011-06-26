using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingPropertyBinderCache : IPropertyBinderCache
    {
        private readonly PropertyBinderCache _inner;
        private readonly IDebugReport _report;

        public RecordingPropertyBinderCache(PropertyBinderCache inner, IDebugReport report)
        {
            _inner = inner;
            _report = report;
        }

        public IPropertyBinder BinderFor(PropertyInfo property)
        {
            var binder = _inner.BinderFor(property);
            _report.AddBindingDetail(new PropertyBinderSelection
                                         {
                                             BinderType = binder.GetType(),
                                             PropertyName = property.Name,
                                             PropertyType = property.PropertyType
                                         });
            return binder;
        }
    }
}