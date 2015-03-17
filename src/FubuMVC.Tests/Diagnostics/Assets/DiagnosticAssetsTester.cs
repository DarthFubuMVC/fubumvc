using System.IO;
using System.Text;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Assets
{
    [TestFixture]
    public class DiagnosticAssetsTester
    {
        private DiagnosticAssetsCache theAssets;

        [SetUp]
        public void SetUp()
        {
            theAssets = new DiagnosticAssetsCache();
        }

        [Test]
        public void reads_embedded_diagnostic_files_for_an_assembly()
        {
            theAssets.For("bootstrap.min.css").ShouldNotBeNull();
            theAssets.For("bootstrap.overrides.css").ShouldNotBeNull();
            theAssets.For("master.css").ShouldNotBeNull();
        }

        [Test]
        public void embedded_file_captures_the_mime_type()
        {
            var file = theAssets.For("master.css");

            file.ContentType.ShouldEqual(MimeType.Css);
        }


        [Test]
        public void embedded_file_captures_the_version()
        {
            var file = theAssets.For("master.css");

            file.Version.ShouldEqual(typeof(IActionBehavior).Assembly.GetName().Version.ToString());
        }

        [Test]
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