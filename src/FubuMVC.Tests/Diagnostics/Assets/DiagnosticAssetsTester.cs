using System.IO;
using System.Reflection;
using System.Text;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Diagnostics.Assets
{
    
    public class DiagnosticAssetsTester
    {
        private DiagnosticAssetsCache theAssets = new DiagnosticAssetsCache(new BehaviorGraph { PackageAssemblies = new Assembly[0] });


        [Fact]
        public void reads_embedded_diagnostic_files_for_an_assembly()
        {
            theAssets.For("bootstrap.min.css").ShouldNotBeNull();
            theAssets.For("bootstrap.overrides.css").ShouldNotBeNull();
            theAssets.For("master.css").ShouldNotBeNull();
        }

        [Fact]
        public void embedded_file_captures_the_mime_type()
        {
            var file = theAssets.For("master.css");

            file.ContentType.ShouldBe(MimeType.Css);
        }


        [Fact]
        public void embedded_file_captures_the_version()
        {
            var file = theAssets.For("master.css");

            file.Version.ShouldBe(typeof(IActionBehavior).Assembly.GetName().Version.ToString());
        }

        [Fact]
        public void embedded_file_can_read_contents()
        {
            var file = theAssets.For("master.css");

            var bytes = file.Contents();
            var text = Encoding.Default.GetString(bytes);

            text.ShouldContain(".filter-container h4, .filter-container .filter");
            text.ShouldContain(".filter-container h4, .filter-container .ui-button-text");
            text.ShouldContain("#filter-dialog button");
        }
    }
}