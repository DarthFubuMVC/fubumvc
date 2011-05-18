using System.ComponentModel;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Urls;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{



    [FubuDiagnostics("Package Loading and Bootstrapping")]
    public class PackageLoadingWriter
    {
        private readonly IUrlRegistry _urls;

        public PackageLoadingWriter(IUrlRegistry urls)
        {
            _urls = urls;
        }

        [FubuDiagnostics("Package Loading and Bootstrapping")]
        public HtmlDocument FullLog()
        {
            var table = LoggingSessionWriter.Write(PackageRegistry.Diagnostics);
            var document = DiagnosticHtml.BuildDocument(_urls, "Full Package Loading Log", table);

            return document;
        }


    }
}