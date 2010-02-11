using System;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class RecordingObjectResolver : IObjectResolver
    {
        private readonly IDebugReport _report;
        private readonly IObjectResolver _resolver;

        public RecordingObjectResolver(IDebugReport report, IObjectResolver resolver)
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
            catch (Exception)
            {
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
    }
}