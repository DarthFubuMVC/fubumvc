using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics
{
    public class DebugCallHandler : IDebugCallHandler
    {
        private readonly IDebugReport _report;
        private readonly IUrlRegistry _urls;
        private readonly IOutputWriter _outputWriter;

        public DebugCallHandler(IDebugReport report, IUrlRegistry urls, IOutputWriter outputWriter)
        {
            _report = report;
            _outputWriter = outputWriter;
            _urls = urls;
        }

        public void Handle()
        {
            var debugWriter = new DebugWriter(_report, _urls);
            var outputWriter = new HttpResponseOutputWriter();
            outputWriter.Write(MimeType.Html.ToString(), debugWriter.Write().ToString());
        }
    }
}