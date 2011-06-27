using System;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingModelBinderCache : IModelBinderCache
    {
        private readonly IModelBinderCache _inner;
        private readonly IDebugReport _report;

        public RecordingModelBinderCache(IModelBinderCache inner, IDebugReport report)
        {
            _inner = inner;
            _report = report;
        }

        public IModelBinder BinderFor(Type modelType)
        {
            var binder = _inner.BinderFor(modelType);
            if (binder != null)
            {
                _report.AddBindingDetail(new ModelBinderSelection
                                             {
                                                 ModelType = modelType,
                                                 BinderType = binder.GetType()
                                             });
            }
            return binder;
        }
    }
}