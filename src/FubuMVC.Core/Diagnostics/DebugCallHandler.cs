using System;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics
{
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
            var debugWriter = new DebugWriter(_report, _urls);
            _writer.Write(MimeType.Html.ToString(), debugWriter.Write().ToString());
        }
    }
}