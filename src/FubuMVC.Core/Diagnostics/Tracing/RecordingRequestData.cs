using FubuCore.Binding;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingRequestData : RequestData
    {
        private readonly IDebugReport _report;

        public RecordingRequestData(IDebugReport report, AggregateDictionary dictionary)
            : base(dictionary)
        {
            _report = report;
        }

        protected override void record(string key, RequestDataSource source, object @object)
        {
            _report.AddBindingDetail(new ModelBindingKey()
            {
                Key = key,
                Source = source,
                Value = @object
            });
        }
    }
}