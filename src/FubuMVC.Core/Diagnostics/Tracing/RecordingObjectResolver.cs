using System;
using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingObjectResolver : IObjectResolver
    {
        private readonly IDebugReport _report;
        private readonly ObjectResolver _resolver;

        public RecordingObjectResolver(IDebugReport report, ObjectResolver resolver)
        {
            _report = report;
            _resolver = resolver;
        }

        public BindResult BindModel(Type type, IRequestData data)
        {
            try
            {
                _report.StartModelBinding(type);
                BindResult result = _resolver.BindModel(type, data);
                _report.EndModelBinding(result.Value);

                return result;
            }
            catch (Exception exc)
            {
                _report.AddBindingDetail(new ModelBindingException
                                             {
                                                 StackTrace = exc.StackTrace
                                             });
                _report.EndModelBinding(null);
                throw;
            }
        }

        public BindResult BindModel(Type type, IBindingContext context)
        {
            try
            {
                _report.StartModelBinding(type);
                BindResult result = _resolver.BindModel(type, context);
                _report.EndModelBinding(result.Value);

                return result;
            }
            catch (Exception)
            {
                _report.EndModelBinding(null);
                throw;
            }
        }

        public void TryBindModel(Type type, IBindingContext context, Action<BindResult> continuation)
        {
            try
            {
                _report.StartModelBinding(type);
                _resolver.TryBindModel(type, context, result =>
                {
                    _report.EndModelBinding(result.Value);
                    continuation(result);
                });
                
            }
            catch (Exception)
            {
                _report.EndModelBinding(null);
                throw;
            }
        }
    }
}