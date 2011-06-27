using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingValueConverterRegistry : IValueConverterRegistry
    {
        private readonly ValueConverterRegistry _inner;
        private readonly IDebugReport _report;

        public RecordingValueConverterRegistry(ValueConverterRegistry inner, IDebugReport report)
        {
            _inner = inner;
            _report = report;
        }

        public ValueConverter FindConverter(PropertyInfo property)
        {
            var converter = _inner.FindConverter(property);
            if (converter != null)
            {
                _report.AddBindingDetail(new ValueConverterSelection
                                             {
                                                 ConverterType = converter.GetType(),
                                                 PropertyName = property.Name,
                                                 PropertyType = property.PropertyType
                                             });
            }
            return converter;
        }
    }
}