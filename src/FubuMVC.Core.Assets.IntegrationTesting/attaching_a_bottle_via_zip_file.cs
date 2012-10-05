using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Core.Assets.IntegrationTesting
{
    [TestFixture]
    public class attaching_a_bottle_via_zip_file : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"
link src/FubuMVC.Core.Assets.IntegrationTesting --clean-all
init src/TestPackage1 pak1
create pak1 -o pak1.zip
");

            installZipPackage("pak1.zip");

        }

        [Test]
        public void read_image_from_a_package()
        {
            endpoints.GetAsset(AssetFolder.images, "icon-add-alt.png")
                .LengthShouldBe(3517)
                .ContentTypeShouldBe(MimeType.Png)
                .StatusCodeShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void read_asset_from_a_bottle()
        {
            endpoints.GetAsset(AssetFolder.scripts, "Pak1-A.js")
                .ContentTypeShouldBe(MimeType.Javascript)
                .ReadAsText().ShouldContain("var name = 'Pak1-A.js';");
        }

    }
}