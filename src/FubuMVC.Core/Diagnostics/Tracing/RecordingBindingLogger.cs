using System;
using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingBindingLogger : IBindingLogger
    {
        private readonly IDebugReport _report;

        public RecordingBindingLogger(IDebugReport report)
        {
            _report = report;
        }

        public void ChoseModelBinder(Type modelType, IModelBinder binder)
        {
            _report.AddBindingDetail(new ModelBinderSelection{
                ModelType = modelType,
                BinderType = binder.GetType()
            });
        }

        public void ChosePropertyBinder(PropertyInfo property, IPropertyBinder binder)
        {
            var selection = new PropertyBinderSelection{
                BinderType = binder.GetType(),
                PropertyName = property.Name,
                PropertyType = property.PropertyType
            };

            _report.AddBindingDetail(selection);
        }

        public void ChoseValueConverter(PropertyInfo property, ValueConverter converter)
        {
            _report
                .AddBindingDetail(new ValueConverterSelection{
                    ConverterType = converter.GetType(),
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType
                });
        }
    }
}