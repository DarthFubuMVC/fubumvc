using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Diagnostics.Runtime
{
    [MoveToDiagnostics]
    public class DebugCallHandler : IDebugCallHandler
    {
        private readonly IDebugReport _report;
        private readonly IUrlRegistry _urls;
        private readonly IOutputWriter _writer;

        public DebugCallHandler(IDebugReport report, IUrlRegistry urls, IOutputWriter writer)
        {
            _report = report;
            _urls = urls;
            _writer = writer;
        }

        public void Handle()
        {
            _writer.Write(MimeType.Html.ToString(), "The debug output has been broken.  Bad Jeremy.  Will be replaced soon.");
        }
    }
}